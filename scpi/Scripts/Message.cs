using System.IO;
using System.Runtime.Intrinsics.Arm;
using System.Xml.Serialization;
using scpi.Pages;

namespace scpi;

public static class MessageManager
{
    public static bool TryReadMessage(Session session, out string message)
    {
        try
        {
            message = ReadMessage(session);
            return true;
        }
        catch (Exception)
        {
            message = string.Empty;
            return false;
        }
    }

    public static string ReadMessage(Session session)
    {
        // Read the message object from the xml file
        Message message = Read(session.Other!, session.User);

        // Get public key of the user
        var publicKey = Session.LoadUserPublicKey(session.Other!);

        // Validate the message
        if (!Validate(message, publicKey))
            throw new InvalidOperationException("Invalid message: signature does not match");

        // Get the symmetric key
        var key = session.ReceiveSymmetricKey();

        // Decrypt the message
        (string text, byte[] iv, _) = AesCipher.DecomposeMessage(message.Text);
        return AesCipher.Decrypt(text, key, iv);
    }

    public static void WriteMessage(string text, Session session)
    {
        // Encrypt the message
        string cipherText = AesCipher.Encrypt(text, session.SymmetricKey, out byte[] iv);
        string composedMessage = AesCipher.ComposeMessage(cipherText, iv, new byte[8]);

        // Sign the message
        string signature = DigitalSignature.Sign(cipherText, session.GetPrivateKey());

        // Save the message object to an xml file
        Write(new Message { Text = composedMessage, Signature = signature },
            session.User, session.Other!);
    }

    private static Message Read(UserDB sender, UserDB receiver)
    {
        var message = DatabaseController.GetMessage(sender, receiver);

        if (message is null)
            throw new InvalidOperationException("Message not found");

        Message m = new Message { Text = message.text, Signature = message.signature };

        return m;
    }

    private static void Write(Message message, UserDB sender, UserDB receiver)
    {
        // Check if message between sender an receiver exists
        var m = DatabaseController.GetMessage(sender, receiver);
        if (m is not null)
        {
            m.text = message.Text;
            m.signature = message.Signature;
            DatabaseController.UpdateMessage(m);
        }
        else DatabaseController.AddMessage(sender, receiver, message.Text, message.Signature);
    }

    private static bool Validate(Message message, byte[] publicKey)
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