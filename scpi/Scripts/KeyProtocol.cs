namespace scpi;

public static class KeyProtocol
{
    public static string KeyToString(byte[] key, string password)
    {
        // Generate a key from the password + key
        string keyString = password + Convert.ToBase64String(key);
        byte[] derivedKey = KeyGenerator.GenerateSymmetricKey(keyString);
        
        // Cipher the key with the derived key using AES
        return AesCipher.Encrypt(Convert.ToBase64String(key), derivedKey, out byte[] iv);
    }
}