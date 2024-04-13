namespace scpi;

/// <summary>
/// This class is used to convert keys to and from strings
/// </summary>
public static class KeyProtocol
{
    /// <summary>
    /// Cipher the key with the derived key using AES
    /// </summary>
    /// <param name="key">The key to convert</param>
    /// <param name="password">The password to use for encryption</param>
    /// <returns> The cypertext of the key</returns>
    private static string KeyToString(byte[] key, string password)
    {
        // Generate a key from the password + key
        string keyString = password;
        byte[] derivedKey = KeyGenerator.GenerateSymmetricKey(keyString, out byte[] salt);
        
        // Cipher the key with the derived key using AES
        var c = AesCipher.Encrypt(Convert.ToBase64String(key), derivedKey, out byte[] iv);
        return AesCipher.ComposeMessage(c, iv, salt);
    }

    /// <summary>
    /// Converts a key to a string
    /// </summary>
    /// <param name="key">The key to convert</param>
    /// <returns>The key as a base64 string</returns>
    private static string KeyToString(byte[] key)
    {
        // Convert the key to a base64 string
        return Convert.ToBase64String(key);
    }

    /// <summary>
    /// Converts the public and private keys to strings
    /// </summary>
    /// <param name="publicKey">The public key to convert</param>
    /// <param name="privateKey">The private key to convert</param>
    /// <param name="password">The password to use for encryption</param>
    /// <returns>The public and private keys as base64 strings</returns>
    public static (string, string) KeysToString(byte[] publicKey, byte[] privateKey, string password)
    {
        // Convert the keys to strings
        return (KeyToString(publicKey), KeyToString(privateKey, password));
    }

    /// <summary>
    /// Decrypt the key using the password
    /// </summary>
    /// <param name="keyString">The key to decrypt</param>
    /// <param name="password">The password to use for decryption</param>
    /// <returns>The decrypted key</returns>
    private static byte[] KeyFromString(string keyString, string password)
    {
        // Decompose the message
        var (cipher_text, iv, salt) = AesCipher.DecomposeMessage(keyString);
        // Decrypt the key
        string key = AesCipher.Decrypt(cipher_text, KeyGenerator.GenerateSymmetricKey(password, salt), iv);
        // Convert the key to a byte array
        return Convert.FromBase64String(key);
    }

    /// <summary>
    /// Converts a key from a string
    /// </summary>
    /// <param name="keyString">The key to convert</param>
    /// <returns>The key as a byte array</returns>
    private static byte[] KeyFromString(string keyString)
    {
        // Convert the key from a base64 string
        return Convert.FromBase64String(keyString);
    }

    /// <summary>
    /// Converts the public and private keys from strings
    /// </summary>
    /// <param name="publicKey">The public key to convert</param>
    /// <param name="privateKey">The private key to convert</param>
    /// <param name="password">The password to use for decryption</param>
    /// <returns>The public and private keys as byte arrays</returns>
    public static (byte[], byte[]) KeysFromString(string publicKey, string privateKey, string password)
    {
        // Convert the keys from strings
        return (KeyFromString(publicKey), KeyFromString(privateKey, password));
    }
}