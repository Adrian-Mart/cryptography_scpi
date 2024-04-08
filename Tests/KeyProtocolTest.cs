namespace scpi.Tests
{
    public class KeyProtocolTests
    {
        [Test]
        public void KeysToString_ReturnsCorrectKeyStrings()
        {
            // Arrange
            (byte[] publicKey, byte[] privateKey) = KeyGenerator.GenerateAsymmetricKeys();
            string password = "password";

            // Act
            (string publicKeyString, string privateKeyString) = KeyProtocol.KeysToString(publicKey, privateKey, password);
            (byte[] publicKeyOut, byte[] privateKeyOut) = KeyProtocol.KeysFromString(publicKeyString, privateKeyString, password);

            // Assert
            // Compare the original keys with the keys that were converted to strings and back
            if (!publicKey.SequenceEqual(publicKeyOut))
            {
                Assert.Fail("Public keys are not equal");
            }
            else if (!privateKey.SequenceEqual(privateKeyOut))
            {
                Assert.Fail("Private keys are not equal");
            }
            else
            {
                Assert.Pass();
            }
        }
    }
}