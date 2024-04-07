using System.Security.Cryptography;

namespace scpi;

public static class RsaCipher
{
    public static string Cipher(byte[] publicKey, string plainText)
    {
        // Encrypt the plain text using RSA
        using (var rsa = RSA.Create())
        {
            rsa.ImportSubjectPublicKeyInfo(publicKey, out _);

            byte[] plainBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            byte[] cipherBytes = rsa.Encrypt(plainBytes, RSAEncryptionPadding.OaepSHA256);
            return Convert.ToBase64String(cipherBytes);
        }
    }

    public static string Decipher(byte[] privateKey, string cipherText)
    {
        // Decrypt the cipher text using RSA
        using (var rsa = RSA.Create())
        {
            rsa.ImportRSAPrivateKey(privateKey, out _);

            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            byte[] plainBytes = rsa.Decrypt(cipherBytes, RSAEncryptionPadding.OaepSHA256);
            return System.Text.Encoding.UTF8.GetString(plainBytes);
        }
    }
}