namespace scpi;

public static class KeyProtocol
{
    private static string KeyToString(byte[] key, string password)
    {
        // Generate a key from the password + key
        string keyString = password;
        byte[] derivedKey = KeyGenerator.GenerateSymmetricKey(keyString, out byte[] salt);
        
        // Cipher the key with the derived key using AES
        var c = AesCipher.Encrypt(Convert.ToBase64String(key), derivedKey, out byte[] iv);
        return AesCipher.ComposeMessage(c, iv, salt);
    }

    private static string KeyToString(byte[] key)
    {
        return Convert.ToBase64String(key);
    }

    public static (string, string) KeysToString(byte[] publicKey, byte[] privateKey, string password)
    {
        return (KeyToString(publicKey), KeyToString(privateKey, password));
    }

    private static byte[] KeyFromString(string keyString, string password)
    {
        var (cipher_text, iv, salt) = AesCipher.DecomposeMessage(keyString);
        // Decrypt the key
        string key = AesCipher.Decrypt(cipher_text, KeyGenerator.GenerateSymmetricKey(password, salt), iv);
        return Convert.FromBase64String(key);
    }

    private static byte[] KeyFromString(string keyString)
    {
        return Convert.FromBase64String(keyString);
    }

    public static (byte[], byte[]) KeysFromString(string publicKey, string privateKey, string password)
    {
        return (KeyFromString(publicKey), KeyFromString(privateKey, password));
    }
}