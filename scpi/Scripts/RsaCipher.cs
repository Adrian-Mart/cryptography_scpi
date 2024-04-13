using System.Security.Cryptography;

namespace scpi;

/// <summary>
/// This class is used to manage RSA encryption
/// </summary>
public static class RsaCipher
{
    /// <summary>
    /// Encrypt the plain text using the public key
    /// </summary>
    /// <param name="publicKey">The public key to encrypt the plain text with</param>
    /// <param name="plainText">The plain text to encrypt</param>
    /// <returns>The cipher text</returns>
    public static string Cipher(byte[] publicKey, string plainText)
    {
        // Encrypt the plain text using RSA
        using (var rsa = RSA.Create())
        {
            rsa.ImportSubjectPublicKeyInfo(publicKey, out _);

            // Convert the plain text to bytes
            byte[] plainBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            // Encrypt the plain bytes
            byte[] cipherBytes = rsa.Encrypt(plainBytes, RSAEncryptionPadding.OaepSHA3_256);
            // Convert the cipher bytes to a base64 string
            return Convert.ToBase64String(cipherBytes);
        }
    }

    /// <summary>
    /// Decrypt the cipher text using the private key
    /// </summary>
    /// <param name="privateKey">The private key to decrypt the cipher text with</param>
    /// <param name="cipherText">The cipher text to decrypt</param>
    /// <returns>The plain text</returns>
    public static string Decipher(byte[] privateKey, string cipherText)
    {
        // Decrypt the cipher text using RSA
        using (var rsa = RSA.Create())
        {
            rsa.ImportRSAPrivateKey(privateKey, out _);
            // Convert the cipher text to bytes
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            // Decrypt the cipher bytes
            byte[] plainBytes = rsa.Decrypt(cipherBytes, RSAEncryptionPadding.OaepSHA3_256);
            // Convert the plain bytes to a string
            return System.Text.Encoding.UTF8.GetString(plainBytes);
        }
    }
}