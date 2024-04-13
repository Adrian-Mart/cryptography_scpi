using System.Security.Cryptography;
using System.Xml.Serialization;

namespace scpi;
public class Session
{
    /// <summary>
    /// The symmetric key used to encrypt and decrypt the session.
    /// </summary>
    public byte[] SymmetricKey { get; private set; }

    /// <summary>
    /// The public key used to encrypt the session.
    /// </summary>
    public byte[]? PublicKey { get; private set; }

    /// <summary>
    /// The private key used to decrypt the session.
    /// </summary>
    public byte[]? PrivateKey { get; private set; }

    /// <summary>
    /// The password used to generate the keys.
    /// </summary>
    public string Password { get; private set; }

    /// <summary>
    /// The user associated with the session.
    /// </summary>
    public UserDB User { get; private set; }

    /// <summary>
    /// The other user associated with the session.
    /// </summary>
    public UserDB? Other { get; private set; }

    /// <summary>
    /// The generated flag.
    /// </summary>
    public bool Generated { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Session"/> class.
    /// </summary>
    /// <param name="user">The user associated with the session</param>
    public Session(string user, string password)
    {
        // Get the user from the database
        var nullableUser = DatabaseController.GetUser(user);

        // Check if the user exists in the database and create it if it does not
        if(nullableUser == null)
        {
            DatabaseController.AddUser(user, password, "");
            User = DatabaseController.GetUser(user) ??
                throw new InvalidOperationException("User not found");
        }
        else User = nullableUser;

        // Get SHA-3 hash of the password
        var hash = SHA3_256.HashData(System.Text.Encoding.UTF8.GetBytes(password));

        // Check if the password is correct and throw an exception if it is not
        if (User.password != Convert.ToBase64String(hash))
            throw new InvalidOperationException("Invalid password");

        Password = password;
        SymmetricKey = KeyGenerator.GenerateSymmetricKey(password);
    }

    /// <summary>
    /// Sets the other user associated with the session.
    /// </summary>
    /// <param name="otherUser">The user associated with the session</param>
    public void SetOtherUser(UserDB otherUser)
    {
        Other = otherUser;
    }

    /// <summary>
    /// Generates the keys for the session.
    /// </summary>
    public void GenerateKeys()
    {
        (PublicKey, PrivateKey) = KeyGenerator.GenerateAsymmetricKeys();
        User.public_key = Convert.ToBase64String(PublicKey);
    }

    /// <summary>
    /// Shares the symmetric key with the other user.
    /// </summary>
    public void ShareSymmetricKey()
    {
        if (Other == null)
            return;

        // Get the public key of the user
        byte[] publicKey = LoadUserPublicKey(Other);

        // Cipher the symmetric key with the user's public key
        string key = Convert.ToBase64String(SymmetricKey);
        string cipheredKey = RsaCipher.Cipher(publicKey, key);

        // Save the shared key in the database or update it if it already exists
        var register = DatabaseController.GetSharedKey(User, Other);
        if (register == null)
            DatabaseController.AddSharedKey(User, Other, cipheredKey);
        else
        {
            register.key = cipheredKey;
            DatabaseController.UpdateSharedKey(register);
        }
    }

    /// <summary>
    /// Receives the symmetric key from the other user.
    /// </summary>
    public byte[] ReceiveSymmetricKey()
    {
        // Check if the other user is set and throw an exception if it is not
        if (Other == null)
            throw new InvalidOperationException("Other user not set");
        // Get the shared key from the database or throw an exception if it is not found
        var register = DatabaseController.GetSharedKey(Other, User) ??
            throw new InvalidOperationException("Shared key not found");
        // Check if the private key is loaded and throw an exception if it is not
        if (PrivateKey == null)
            throw new InvalidOperationException("Private key not loaded");

        // Decipher the symmetric key with the user's private key
        string key = RsaCipher.Decipher(PrivateKey, register.key);
        return Convert.FromBase64String(key);
    }

    /// <summary>
    /// Saves the session keys in a secure manner.
    /// </summary>
    /// <returns>The keys to be saved in a file</returns>
    public (string, string) SaveKeys()
    {
        // Check if the keys are loaded and throw an exception if they are not
        if (PrivateKey == null || PublicKey == null)
            throw new InvalidOperationException("Asymmetric keys are null");
        // Convert the keys to strings
        (string publicKey, string privateKey) =
            KeyProtocol.KeysToString(PublicKey, PrivateKey, Password);

        User.public_key = publicKey;
        // Update the user in the database or throw an exception if it is not found
        if (DatabaseController.GetUser(User.user) == null)
            throw new InvalidOperationException("User not found");
        else
            DatabaseController.UpdateUser(User);
        // Return the keys to be saved in a file
        return (KeyToXML(publicKey), KeyToXML(privateKey));
    }

    /// <summary>
    /// Loads the session keys from a secure manner.
    /// </summary>
    /// <param name="privkey">The private key to load</param>
    /// <param name="pubKey">The public key to load</param>
    public void LoadKeys(string privkey, string pubKey)
    {
        // Load the public key
        string pub = KeyFromXML(pubKey);
        // Load the private key
        string priv = KeyFromXML(privkey);

        // Convert the keys from strings to byte arrays
        (PublicKey, PrivateKey) = KeyProtocol.KeysFromString(pub, priv, Password);
    }

    /// <summary>
    /// Reads a session key from an xml file content.
    /// </summary>
    /// <param name="content">The content of the xml file</param>
    /// <returns>The session key read from the xml file</returns>
    private string KeyFromXML(string content)
    {
        // Read a SessionKey object from the xml file content
        var serializer = new XmlSerializer(typeof(SessionKey));
        using (var reader = new StringReader(content))
        {
            // Deserialize the session key from the xml file content or throw an exception if it is invalid
            var key = serializer.Deserialize(reader) as SessionKey ??
                throw new InvalidOperationException("Invalid session key");
            // Return the value of the session key
            return key.Value;
        }
    }

    /// <summary>
    /// Writes a session key to an xml file content.
    /// </summary>
    /// <param name="key">The session key to write</param>
    /// <returns>The content of the xml file</returns>
    private string KeyToXML(string key)
    {
        // Write a SessionKey object to the xml file content
        var serializer = new XmlSerializer(typeof(SessionKey));
        using (var writer = new StringWriter())
        {
            // Serialize the session key to the xml file content
            serializer.Serialize(writer, new SessionKey { Value = key });
            // Return the content of the xml file
            return writer.ToString();
        }
    }

    /// <summary>
    /// Loads the public key of a user from the database.
    /// </summary>
    /// <param name="user">The user to load the public key from</param>
    /// <returns>The public key of the user</returns>
    public static byte[] LoadUserPublicKey(UserDB user)
    {
        // Get the user from the database or throw an exception if it is not found
        var register = DatabaseController.GetUser(user.user) ??
            throw new InvalidOperationException("User not found");
        // Check if the public key is loaded and throw an exception if it is not
        return Convert.FromBase64String(register.public_key!);
    }

    /// <summary>
    /// Gets the public key of the session.
    /// </summary>
    /// <returns>The public key of the session</returns>
    public byte[] GetPublicKey()
    {
        // Check if the public key is loaded and throw an exception if it is not
        if (PublicKey == null)
            throw new InvalidOperationException("Public key not loaded");
        // Return the public key of the session
        return PublicKey;
    }

    /// <summary>
    /// Gets the private key of the session.
    /// </summary>
    /// <returns>The private key of the session</returns>
    public byte[] GetPrivateKey()
    {
        // Check if the private key is loaded and throw an exception if it is not
        if (PrivateKey == null)
            throw new InvalidOperationException("Private key not loaded");
        // Return the private key of the session
        return PrivateKey;
    }

    /// <summary>
    /// Gets the user associated with the session.
    /// </summary>
    public string GetOtherUser() => Other!.user;

    /// <summary>
    /// Reads a message from the session.
    /// </summary>
    /// <returns>The message read from the session or a message indicating that there are no messages</returns>
    public string ReadMessage()
    {
        try
        {
            return MessageManager.ReadMessage(this);
        }
        catch (Exception)
        {
            return "[Sin mensajes]";
        }
    }

    /// <summary>
    /// Reads a cipher from the session.
    /// </summary>
    /// <returns>The cipher read from the session or a message indicating that there are no messages</returns>
    public string ReadCipher()
    {
        try
        {
            return MessageManager.ReadCipher(this);
        }
        catch (Exception)
        {
            return "[Sin mensajes]";
        }
    }

    /// <summary>
    /// Decrypts a cipher from the session.
    /// </summary>
    /// <param name="cipher">The cipher to decrypt</param>
    /// <returns>The message that was sent by the other user</returns>
    public string Decipher(string cipher)
    {
        try
        {
            return MessageManager.Decipher(cipher, this);
        }
        catch (Exception)
        {
            return "[Sin mensajes]";
        }
    }

    /// <summary>
    /// Sends a message to the other user.
    /// </summary>
    /// <param name="message">The message to send</param>
    public void SendMessage(string message)
    {
        MessageManager.WriteMessage(message, this);
    }
}

/// <summary>
/// A session key.
/// </summary>
public class SessionKey
{
    /// <summary>
    /// The value of the session key.
    /// </summary>
    public required string Value { get; set; }

    /// <summary>
    /// Gets the public key of the session key.
    /// </summary>
    public byte[] GetPublicKey()
    {
        return Convert.FromBase64String(Value);
    }
}