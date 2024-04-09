using System.IO;
using System.Runtime.Intrinsics.Arm;
using System.Xml.Serialization;
using scpi.Pages;

namespace scpi;

public class MessageManager
{
    public string BasePath { get; set; }

    public MessageManager(string path)
    {
        BasePath = path;
    }

    public bool TryReadMessage(Session session, string user, out string message)
    {
        try
        {
            message = ReadMessage(session, user);
            return true;
        }
        catch (Exception)
        {
            message = string.Empty;
            return false;
        }
    }

    public string ReadMessage(Session session, string senderUsername)
    {
        // Read the message object from the xml file
        Message message = Read(senderUsername, session.User);

        // Get public key of the user
        var publicKey = Session.LoadUserPublicKey(senderUsername, BasePath);

        // Validate the message
        if (!Validate(message, publicKey))
            throw new InvalidOperationException("Invalid message: signature does not match");

        // Get the symmetric key
        var key = session.ReceiveSymmetricKey(BasePath, senderUsername);

        // Decrypt the message
        (string text, byte[] iv, _) = AesCipher.DecomposeMessage(message.Text);
        return AesCipher.Decrypt(text, key, iv);
    }

    public void WriteMessage(string text, Session session, string receiver)
    {
        // Encrypt the message
        string cipherText = AesCipher.Encrypt(text, session.SymmetricKey, out byte[] iv);
        string composedMessage = AesCipher.ComposeMessage(cipherText, iv, new byte[8]);

        // Sign the message
        string signature = DigitalSignature.Sign(cipherText, session.GetPrivateKey());

        // Save the message object to an xml file
        Write(new Message { Text = composedMessage, Signature = signature },
            session.User, receiver);
    }

    private Message Read(string senderUsername, string receiverUsername)
    {
        UserDB receiver = DatabaseController.GetUser(receiverUsername) ??
            throw new InvalidOperationException("Receiver not found");
        UserDB sender = DatabaseController.GetUser(senderUsername) ??
            throw new InvalidOperationException("Sender not found");

        var message = DatabaseController.GetMessage(sender, receiver);

        if (message is null)
            throw new InvalidOperationException("Message not found");

        Message m = new Message { Text = message.Text, Signature = message.Signature };

        return m;
    }

    private void Write(Message message, string senderUsername, string receiverUsername)
    {
        UserDB receiver = DatabaseController.GetUser(receiverUsername) ??
            throw new InvalidOperationException("Receiver not found");
        UserDB sender = DatabaseController.GetUser(senderUsername) ??
            throw new InvalidOperationException("Sender not found");

        DatabaseController.AddMessage(sender, receiver, message.Text, message.Signature);
    }

    private bool Validate(Message message, byte[] publicKey)
    {
        (string text, _, _) = AesCipher.DecomposeMessage(message.Text);
        // Validate the message
        return DigitalSignature.Verify(text, message.Signature, publicKey);
    }
}

public class Message
{
    public required string Text { get; set; }
    public required string Signature { get; set; }

}