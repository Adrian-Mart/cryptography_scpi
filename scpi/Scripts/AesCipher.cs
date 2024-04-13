using System.Security.Cryptography;

namespace scpi;

/// <summary>
/// This class is used to encrypt and decrypt messages using AES
/// </summary>
public static class AesCipher
{
    /// <summary>
    /// Encrypts a string using AES
    /// </summary>
    /// <param name="plainText">The text to encrypt</param>
    /// <param name="Key">The key to use for encryption</param>
    /// <param name="IV">The IV to use for encryption</param>
    /// <returns>The encrypted text</returns>
    public static string Encrypt(string plainText, byte[] Key, out byte[] IV)
    {
        // Generate a secure IV for the encryption
        IV = GenerateSecureIV();

        // Create an AES object
        using (Aes aesAlg = Aes.Create())
        {
            // Set the key
            aesAlg.Key = Key;
            // Set the IV
            aesAlg.IV = IV;

            // Create an encryptor object using the key and IV
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            
            // Create a memory stream to store the encrypted data 
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                // Create a crypto stream to write the encrypted data to the memory stream
                // using the encryptor object created above, the memory stream, and write mode
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    // Create a stream writer to write the plain text to the crypto stream
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        // Write the plain text to the crypto stream
                        swEncrypt.Write(plainText);
                    }
                    // Return the encrypted data as a base64 string
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }
    }

    /// <summary>
    /// Composes a message from the cipher text, IV, and salt
    /// </summary>
    /// <param name="cipher_text">The cipher text</param>
    /// <param name="IV">The IV</param>
    /// <param name="salt">The salt</param>
    /// <returns>The composed message</returns>
    public static string ComposeMessage(string cipher_text, byte[] IV, byte[] salt)
    {
        // Get the iv as a string
        var iv_string = Convert.ToBase64String(IV);
        // Get the salt as a string
        var salt_string = Convert.ToBase64String(salt);
        // Join the text, iv length, and iv
        return $"{iv_string}:{salt_string}:{cipher_text}";
    }

    /// <summary>
    /// Decomposes a message into its parts
    /// </summary>
    /// <param name="message">The message to decompose</param>
    /// <returns>The cipher text, IV, and salt</returns>
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

    /// <summary>
    /// Generates a secure IV for encryption
    /// </summary>
    /// <returns>The IV</returns>
    private static byte[] GenerateSecureIV()
    {
        // Create an AES object
        using (Aes aes = Aes.Create())
        {
            // Generate a secure IV
            aes.GenerateIV();
            // Return the IV
            return aes.IV;
        }
    }

    /// <summary>
    /// Decrypts a string using AES
    /// </summary>
    /// <param name="cipherText">The text to decrypt</param>
    /// <param name="Key">The key to use for decryption</param>
    /// <param name="IV">The IV to use for decryption</param>
    /// <returns>The decrypted text</returns>
    public static string Decrypt(string cipherText, byte[] Key, byte[] IV)
    {
        // Create an AES object
        using (Aes aesAlg = Aes.Create())
        {
            // Set the key
            aesAlg.Key = Key;
            // Set the IV
            aesAlg.IV = IV;

            // Create a decryptor object using the key and IV
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            // Create a memory stream to store the encrypted data using the cipher text as a byte array
            using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
            {
                // Create a crypto stream to read the encrypted data from the memory stream
                // using the decryptor object created above, the memory stream, and read mode
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    // Create a stream reader to read the decrypted data from the crypto stream
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        // Return the decrypted data as a string
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }
    }
}