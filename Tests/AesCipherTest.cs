namespace scpi.Tests
{
    public class AesCipherTests
    {
        [Test]
        public void Cipher_Decipher_Econding()
        {
            // Arrange
            string plainText = "Hello, world!";
            string password = "myPassword";
            byte[] key = KeyGenerator.GenerateSymmetricKey(password);

            // Act
            string cipherText = AesCipher.Encrypt(plainText, key, out byte[] iv);
            string ComposeMessage = AesCipher.ComposeMessage(cipherText, iv);
            (string cipherText2, byte[] iv2) = AesCipher.DecomposeMessage(ComposeMessage);
            string decipheredText = AesCipher.Decrypt(cipherText2, key, iv2);

            // Assert
            if (decipheredText != plainText)
                Assert.Fail("Deciphered text does not match the plain text.");
            else
                Assert.Pass();
        }
    }
}