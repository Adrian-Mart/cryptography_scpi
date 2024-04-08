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
            byte[] key = KeyGenerator.GenerateSymmetricKey(password, out byte[] salt);

            // Act
            string cipherText = AesCipher.Encrypt(plainText, key, out byte[] iv);
            string ComposeMessage = AesCipher.ComposeMessage(cipherText, iv, salt);
            (string cipherText2, byte[] iv2, byte[] salt2) = AesCipher.DecomposeMessage(ComposeMessage);
            byte[] key2 = KeyGenerator.GenerateSymmetricKey(password, salt2);
            string decipheredText = AesCipher.Decrypt(cipherText2, key2, iv2);

            // Assert
            if (decipheredText != plainText)
                Assert.Fail("Deciphered text does not match the plain text.");
            else
                Assert.Pass();
        }
    }
}