namespace scpi.Tests
{
    public class DigitalSignatureTests
    {
        [Test]
        public void Verify_ReturnsTrueForValidSignature()
        {
            // Arrange
            string message = "Hello, world!";
            (byte[] publicKey, byte[] privateKey) = KeyGenerator.GenerateAsymmetricKeys();
            string signature = DigitalSignature.Sign(message, privateKey, publicKey);

            // Act
            bool result = DigitalSignature.Verify(message, signature, publicKey);

            // Assert
            if (result)
                Assert.Pass();
            else
                Assert.Fail("Signature was valid but Verify returned false.");
        }

        [Test]
        public void Verify_ReturnsFalseForInvalidSignature()
        {
            // Arrange
            string message = "Hello, world!";
            (byte[] publicKey, byte[] privateKey) = KeyGenerator.GenerateAsymmetricKeys();
            string signature = DigitalSignature.Sign(message, privateKey, publicKey);

            // Act
            string message2 = "Hello, world?";
            bool result = DigitalSignature.Verify(message2, signature, publicKey);

            // Assert
            if (result)
                Assert.Fail("Signature was invalid but Verify returned true.");
            else
                Assert.Pass();
        }
    }
}