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

    public string ReadMessage(Session session, string user)
    {
        var fullPath = Path.Combine(BasePath, $"{user}_message.xml");

        // Read the message object from the xml file
        Message message = Read(fullPath);

        // Get public key of the user
        var publicKey = Session.LoadUserPublicKey(user, BasePath);

        // Validate the message
        if (!Validate(message, publicKey))
            throw new InvalidOperationException("Invalid message: signature does not match");

        // Get the symmetric key
        var key = session.ReceiveSymmetricKey(BasePath, user);

        // Decrypt the message
        (string text, byte[] iv, _) = AesCipher.DecomposeMessage(message.Text);
        return AesCipher.Decrypt(text, key, iv);
    }

    public void WriteMessage(string text, Session session)
    {
        var fullPath = Path.Combine(BasePath, $"{session.User}_message.xml");

        // Encrypt the message
        string cipherText = AesCipher.Encrypt(text, session.SymmetricKey, out byte[] iv);
        string composedMessage = AesCipher.ComposeMessage(cipherText, iv, new byte[8]);

        // Sign the message
        string signature = DigitalSignature.Sign(cipherText, session.GetPrivateKey());

        // Save the message object to an xml file
        Write(new Message { Text = composedMessage, Signature = signature }, fullPath);
    }

    private Message Read(string path)
    {
        // Read the message object from the xml file
        XmlSerializer serializer = new XmlSerializer(typeof(Message));
        using var reader = new StreamReader(path);
        return serializer.Deserialize(reader) as Message ??
            throw new InvalidOperationException("Invalid message");
    }

    private void Write(Message message, string path)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Message));

        using var writer = new StreamWriter(path);
        serializer.Serialize(writer, message);
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