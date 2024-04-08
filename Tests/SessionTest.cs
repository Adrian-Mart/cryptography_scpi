using NUnit.Framework;

namespace scpi.Tests
{
    public class SessionTests
    {

        [Test]
        public void ShareSymmetricKey_WithValidSessionAndUser_SharesSymmetricKey()
        {
            string generalPath = Directory.GetCurrentDirectory() + "/session_data/";
            // Clean up
            // Delete all files in the general path
            DirectoryInfo di = new DirectoryInfo(generalPath);
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }

            // Arrange
            // Session for user1
            Session session1 = new Session("user1", "password1");
            session1.GenerateKeys();

            // Session for user2
            Session session2 = new Session("user2", "password2");
            session2.GenerateKeys();
            // Save keys for user2
            session2.SaveKeys(generalPath);

            // Act
            // Share user1 symmetric key with user2
            session1.ShareSymmetricKey(generalPath, "user2");
            // Get user1 symmetric key
            byte[] symmetricKey = session2.ReceiveSymmetricKey(generalPath, "user1");

            // Assert

            // If symmetric key is equal to user1 symmetric key, the test passes
            if (symmetricKey.SequenceEqual(session1.SymmetricKey))
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail("Symmetric keys are not equal");
            }

            
        }
    }
}