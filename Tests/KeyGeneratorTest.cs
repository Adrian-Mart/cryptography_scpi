using System.Security.Cryptography;
using NUnit.Framework;

namespace scpi.Tests
{
    public class KeyGeneratorTests
    {

        [Test]
        public void GenerateSymmetricKey_ValidPassword_ReturnsKey()
        {
            // Arrange
            string password = "myPassword";

            // Act
            byte[] key = KeyGenerator.GenerateSymmetricKey(password);

            // Assert
            string plainText = "Hello, world!";
            string cipherText = CipherAES(key, plainText);
            string decipheredText = DecipherAES(key, cipherText);
            if (decipheredText != plainText)
                Assert.Fail("Deciphered text does not match the plain text.");
            else
                Assert.Pass();
        }

        [Test]
        public void GenerateAsymmetricKeys_ReturnsKeyPair()
        {
            // Act
            (byte[] publicKey, byte[] privateKey) = KeyGenerator.GenerateAsymmetricKeys();

            // Assert
            string plainText = "Hello, world!";
            string cipherText = CipherRSA(publicKey, plainText);
            string decipheredText = DecipherRSA(privateKey, cipherText);
            if (decipheredText != plainText)
                Assert.Fail("Deciphered text does not match the plain text.");
            else
                Assert.Pass();
        }

        private string CipherAES(byte[] key, string plainText)
        {
            // Encrypt the plain text using AES-256
            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.GenerateIV();

                using (var encryptor = aes.CreateEncryptor())
                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    byte[] plainBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
                    cs.Write(plainBytes, 0, plainBytes.Length);
                    cs.FlushFinalBlock();
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        private string DecipherAES(byte[] key, string cipherText)
        {
            // Decrypt the cipher text using AES-256
            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.GenerateIV();

                using (var decryptor = aes.CreateDecryptor())
                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                {
                    byte[] cipherBytes = Convert.FromBase64String(cipherText);
                    cs.Write(cipherBytes, 0, cipherBytes.Length);
                    cs.FlushFinalBlock();
                    return System.Text.Encoding.UTF8.GetString(ms.ToArray());
                }
            }
        }

        private string CipherRSA(byte[] publicKey, string plainText)
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

        private string DecipherRSA(byte[] privateKey, string cipherText)
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
}