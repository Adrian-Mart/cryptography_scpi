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
    public string User { get; private set; }

    private byte[] salt;

    /// <summary>
    /// Initializes a new instance of the <see cref="Session"/> class.
    /// </summary>
    public Session(string user, string password)
    {
        User = user;
        Password = password;
        SymmetricKey = KeyGenerator.GenerateSymmetricKey(password, out salt);
    }

    
    public void GenerateKeys()
    {
        (PublicKey, PrivateKey) = KeyGenerator.GenerateAsymmetricKeys();
    }

    public void ShareSymmetricKey(string path, string user)
    {
        string fileName = $"{User}_{user}_key.xml";
        var fullPath = Path.Combine(path, fileName);

        // Get the public key of the user
        byte[] publicKey = LoadUserPublicKey(user, path);

        // Cipher the symmetric key with the user's public key
        string key = Convert.ToBase64String(SymmetricKey);
        string cipheredKey = RsaCipher.Cipher(publicKey, key);

        // Write the symmetric key to the xml file
        WriteSessionKey(cipheredKey, fullPath);
    }

    public byte[] ReceiveSymmetricKey(string path, string user)
    {
        string fileName = $"{user}_{User}_key.xml";
        var fullPath = Path.Combine(path, fileName);

        // Load the ciphered symmetric key from the xml file
        string cipheredKey = ReadSessionKey(fullPath).Value;

        if(PrivateKey == null)
            throw new InvalidOperationException("Private key not loaded");

        // Decipher the symmetric key with the user's private key
        string key = RsaCipher.Decipher(PrivateKey, cipheredKey);
        return Convert.FromBase64String(key);
    }

    /// <summary>
    /// Saves the session keys in a secure manner.
    /// </summary>
    public void SaveKeys(string path)
    {
        if(PrivateKey == null || PublicKey == null)
            throw new InvalidOperationException("Asymmetric keys are null");

        (string publicKey, string privateKey) = KeyProtocol.KeysToString(PublicKey, PrivateKey, Password);

        string publicPath = Path.Combine(path, $"{User}_pub_key.xml");
        string privatePath = Path.Combine(path, $"{User}_priv_key.xml");

        // Save the public key
        WriteSessionKey(publicKey, publicPath);
        // Save the private key
        WriteSessionKey(privateKey, privatePath);
    }

    public void LoadKeys(string path)
    {
        string publicPath = Path.Combine(path, $"{User}_pub_key.xml");
        string privatePath = Path.Combine(path, $"{User}_priv_key.xml");

        // Load the public key
        string pub = ReadSessionKey(publicPath).Value;
        // Load the private key
        string priv = ReadSessionKey(privatePath).Value;

        // Convert the keys from strings to byte arrays
        (PublicKey, PrivateKey) = KeyProtocol.KeysFromString(pub, priv, Password);
    }

    public byte[] LoadUserPublicKey(string user, string path)
    {
        string fileName = $"{user}_pub_key.xml";
        var fullPath = Path.Combine(path, fileName);

        // Read the SessionKey from the xml file
        return ReadSessionKey(fullPath).GetPublicKey();
    }

    private SessionKey ReadSessionKey(string path)
    {
        // Read the SessionKey from the xml file
        XmlSerializer serializer = new XmlSerializer(typeof(SessionKey));
        using var reader = new StreamReader(path);
        return serializer.Deserialize(reader) as SessionKey??
            throw new InvalidOperationException("Invalid session key");
    }

    private void WriteSessionKey(string key, string path)
    {   
        SessionKey sessionKey = new SessionKey { Value = key };
        XmlSerializer serializer = new XmlSerializer(typeof(SessionKey));
        
        using var writer = new StreamWriter(path);
        serializer.Serialize(writer, sessionKey);
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