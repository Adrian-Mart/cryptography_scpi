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

    public UserDB? Other { get; private set; }

    public bool Generated { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Session"/> class.
    /// </summary>
    public Session(string user, string password)
    {
        
        var nullableUser = DatabaseController.GetUser(user);

        if(nullableUser == null)
        {
            DatabaseController.AddUser(user, password, "");
            User = DatabaseController.GetUser(user) ??
                throw new InvalidOperationException("User not found");
        }
        else User = nullableUser;

        // Get SHA-3 hash of the password
        var hash = SHA3_256.HashData(System.Text.Encoding.UTF8.GetBytes(password));
        if (User.password != Convert.ToBase64String(hash))
            throw new InvalidOperationException("Invalid password");

        Password = password;
        SymmetricKey = KeyGenerator.GenerateSymmetricKey(password);
    }

    public void SetOtherUser(UserDB otherUser)
    {
        Other = otherUser;
    }

    public void GenerateKeys()
    {
        (PublicKey, PrivateKey) = KeyGenerator.GenerateAsymmetricKeys();
    }

    public void ShareSymmetricKey()
    {
        if (Other == null)
            return;

        // Get the public key of the user
        byte[] publicKey = LoadUserPublicKey(Other);

        // Cipher the symmetric key with the user's public key
        string key = Convert.ToBase64String(SymmetricKey);
        string cipheredKey = RsaCipher.Cipher(publicKey, key);

        var register = DatabaseController.GetSharedKey(User, Other);
        if (register == null)
            DatabaseController.AddSharedKey(User, Other, cipheredKey);
        else
        {
            register.key = cipheredKey;
            DatabaseController.UpdateSharedKey(register);
        }
    }

    public byte[] ReceiveSymmetricKey()
    {
        if (Other == null)
            throw new InvalidOperationException("Other user not set");

        var register = DatabaseController.GetSharedKey(Other, User) ??
            throw new InvalidOperationException("Shared key not found");

        if (PrivateKey == null)
            throw new InvalidOperationException("Private key not loaded");

        // Decipher the symmetric key with the user's private key
        string key = RsaCipher.Decipher(PrivateKey, register.key);
        return Convert.FromBase64String(key);
    }

    /// <summary>
    /// Saves the session keys in a secure manner.
    /// </summary>
    public (string, string) SaveKeys()
    {
        if (PrivateKey == null || PublicKey == null)
            throw new InvalidOperationException("Asymmetric keys are null");

        (string publicKey, string privateKey) =
            KeyProtocol.KeysToString(PublicKey, PrivateKey, Password);

        User.public_key = publicKey;

        if (DatabaseController.GetUser(User.user) == null)
            throw new InvalidOperationException("User not found");
        else
            DatabaseController.UpdateUser(User);

        return (KeyToXML(publicKey), KeyToXML(privateKey));
    }

    public void LoadKeys(string privkey, string pubKey)
    {
        // Load the public key
        string pub = KeyFromXML(pubKey);
        // Load the private key
        string priv = KeyFromXML(privkey);

        // Convert the keys from strings to byte arrays
        (PublicKey, PrivateKey) = KeyProtocol.KeysFromString(pub, priv, Password);
    }

    private string KeyFromXML(string content)
    {
        // Read a SessionKey object from the xml file content
        var serializer = new XmlSerializer(typeof(SessionKey));
        using (var reader = new StringReader(content))
        {
            var key = serializer.Deserialize(reader) as SessionKey ??
                throw new InvalidOperationException("Invalid session key");

            return key.Value;
        }
    }

    private string KeyToXML(string key)
    {
        // Write a SessionKey object to the xml file content
        var serializer = new XmlSerializer(typeof(SessionKey));
        using (var writer = new StringWriter())
        {
            serializer.Serialize(writer, new SessionKey { Value = key });
            return writer.ToString();
        }
    }

    public static byte[] LoadUserPublicKey(UserDB user)
    {
        var register = DatabaseController.GetUser(user.user) ??
            throw new InvalidOperationException("User not found");

        return Convert.FromBase64String(register.public_key!);
    }

    public byte[] GetPublicKey()
    {
        if (PublicKey == null)
            throw new InvalidOperationException("Public key not loaded");

        return PublicKey;
    }

    public byte[] GetPrivateKey()
    {
        if (PrivateKey == null)
            throw new InvalidOperationException("Private key not loaded");

        return PrivateKey;
    }

    public string GetOtherUser() => Other!.user;

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

    public void SendMessage(string message)
    {
        MessageManager.WriteMessage(message, this);
    }
}

public class SessionKey
{
    public required string Value { get; set; }

    public byte[] GetPublicKey()
    {
        return Convert.FromBase64String(Value);
    }
}