using NUnit.Framework;

namespace scpi.Tests
{
    public class RsaCipherTests
    {
        [Test]
        public void CipherAndDecipher_WithPublicKeyAndPrivateKey_ReturnsOriginalPlainText()
        {
            // Arrange
            string plainText = "Hello, world!";
            (byte[] publicKey, byte[] privateKey) = KeyGenerator.GenerateAsymmetricKeys();

            // Act
            string cipherText = RsaCipher.Cipher(publicKey, plainText);
            string decipheredText = RsaCipher.Decipher(privateKey, cipherText);

            // Assert
            if (plainText == decipheredText)
                Assert.Pass();
            else
                Assert.Fail("Deciphered text did not match original text.");
        }
    }
}