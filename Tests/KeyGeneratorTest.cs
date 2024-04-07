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
            var iv = GenerateSecureIV();
            string cipherText = EncryptStringAes(plainText, key, iv);
            string decipheredText = DecryptStringAes(cipherText, key, iv);
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

        public static string EncryptStringAes(string plainText, byte[] Key, byte[] IV)
        {
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

        public static byte[] GenerateSecureIV()
        {
            using (Aes aes = Aes.Create())
            {
                aes.GenerateIV();
                return aes.IV;
            }
        }

        public static string DecryptStringAes(string cipherText, byte[] Key, byte[] IV)
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