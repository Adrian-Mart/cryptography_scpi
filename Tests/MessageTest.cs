using NUnit.Framework;

namespace scpi.Tests
{
    public class MessageManagerTests
    {
        [Test]
        public void MessageTest()
        {
            string generalPath = Directory.GetCurrentDirectory() + "/message_data/";

            // Clean up
            DirectoryInfo di = new DirectoryInfo(generalPath);
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }

            string[] testMessages = new string[]
            {
                "Test message 1",
                "Test message 2",
                "Test message 3",
                "Test message 4"
            };

            // Arrange
            // Session for user1
            Session session1 = new Session("user1", "password1");
            session1.GenerateKeys();
            // Save keys for user1
            session1.SaveKeys(generalPath);

            // Session for user2
            Session session2 = new Session("user2", "password2");
            session2.GenerateKeys();
            // Save keys for user2
            session2.SaveKeys(generalPath);

            // Share user1 symmetric key with user2
            session1.ShareSymmetricKey(generalPath, "user2");
            // Share user2 symmetric key with user1
            session2.ShareSymmetricKey(generalPath, "user1");

            // Create message manager
            var messageManager = new MessageManager(generalPath);

            // Act
            int i = 0;
            foreach (var m in testMessages)
            {
                Session sender = i % 2 == 0 ? session1 : session2;
                Session receiver = i % 2 == 0 ? session2 : session1;

                messageManager.WriteMessage(m, sender);
                string decryptedText = messageManager.ReadMessage(receiver, sender.User);
                
                if (m != decryptedText)
                    Assert.Fail("Decrypted text does not match the original message");

                i++;
            }

            Assert.Pass();
        }
    }
}