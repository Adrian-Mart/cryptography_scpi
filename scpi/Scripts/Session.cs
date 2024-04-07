namespace scpi;
public class Session
{
    /// <summary>
    /// The symmetric key used to encrypt and decrypt the session.
    /// </summary>
    public byte[]? SymmetricKey { get; private set; }

    /// <summary>
    /// The public key used to encrypt the session.
    /// </summary>
    public string? PublicKey { get; private set; }

    /// <summary>
    /// The private key used to decrypt the session.
    /// </summary>
    public string? PrivateKey { get; private set; }

    /// <summary>
    /// The password used to generate the keys.
    /// </summary>
    public string Password { get; private set; }

    /// <summary>
    /// The user associated with the session.
    /// </summary>
    public string User { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Session"/> class.
    /// </summary>
    public Session(string user, string password)
    {
        User = user;
        Password = password;

        throw new NotImplementedException();
    }

    /// <summary>
    /// Generates the session keys using the password.
    /// </summary>
    public void GenerateKeys(string password)
    {
        SymmetricKey = KeyGenerator.GenerateSymmetricKey(password);
        (PublicKey, PrivateKey) = KeyGenerator.GenerateAsymmetricKeys();
    }


    /// <summary>
    /// Saves the session keys in a secure manner.
    /// </summary>
    public void SaveKeys()
    {
        // Remove this line and implement the method
        throw new NotImplementedException();
    }
    
    public void LoadKeys()
    {
        // Remove this line and implement the method
        throw new NotImplementedException();
    }
}
