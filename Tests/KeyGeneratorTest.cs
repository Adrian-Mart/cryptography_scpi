using System.Security.Cryptography;

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
            string cipherText = AesCipher.Encrypt(plainText, key, out byte[] iv);
            string decipheredText = AesCipher.Decrypt(cipherText, key, iv);
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
            string cipherText = RsaCipher.Cipher(publicKey, plainText);
            string decipheredText = RsaCipher.Decipher(privateKey, cipherText);
            if (decipheredText != plainText)
                Assert.Fail("Deciphered text does not match the plain text.");
            else
                Assert.Pass();
        }
    }
}