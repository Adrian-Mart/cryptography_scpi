using System.Security.Cryptography;

namespace scpi;

public static class AesCipher
{
    public static string Encrypt(string plainText, byte[] Key, out byte[] IV)
    {
        IV = GenerateSecureIV();
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Key;
            aesAlg.IV = IV;

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }
    }

    public static string ComposeMessage(string cipher_text, byte[] IV, byte[] salt)
    {
        // Get the iv as a string
        var iv_string = Convert.ToBase64String(IV);
        // Get the salt as a string
        var salt_string = Convert.ToBase64String(salt);
        // Join the text, iv length, and iv
        return $"{iv_string}:{salt_string}:{cipher_text}";
    }

    public static (string, byte[], byte[]) DecomposeMessage(string message)
    {
        // Split the message into its parts
        var parts = message.Split(':');
        // Get the iv
        var iv = Convert.FromBase64String(parts[0]);
        // Get the salt
        var salt = Convert.FromBase64String(parts[1]);
        // Get the cipher text
        var cipher_text = parts[2];
        // Return the parts
        return (cipher_text, iv, salt);
    }

    private static byte[] GenerateSecureIV()
    {
        using (Aes aes = Aes.Create())
        {
            aes.GenerateIV();
            return aes.IV;
        }
    }

    public static string Decrypt(string cipherText, byte[] Key, byte[] IV)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Key;
            aesAlg.IV = IV;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }
    }
}