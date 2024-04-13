using System.IO;
using System.Runtime.Intrinsics.Arm;
using System.Xml.Serialization;
using scpi.Pages;

namespace scpi;

/// <summary>
/// This class is used to manage messages
/// </summary>
public static class MessageManager
{
    /// <summary>
    /// Try to read a message from the session
    /// </summary>
    /// <param name="session">The session to read the message from</param>
    /// <param name="message">The message that was read</param>
    /// <returns>True if the message was read successfully, false otherwise</returns>
    public static bool TryReadMessage(Session session, out string message)
    {
        // Try to read the message
        try
        {
            // Read the message
            message = ReadMessage(session);
            return true;
        }
        catch (Exception)
        {
            // Return false if the message could not be read
            message = string.Empty;
            return false;
        }
    }

    /// <summary>
    /// Decrypt a message from the session
    /// </summary>
    /// <param name="session">The session to read the message from</param>
    /// <returns>The message that was sent by the other user</returns>
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

    /// <summary>
    /// Validate a message from the session
    /// </summary>
    /// <param name="session">The session to read the message from</param>
    /// <returns>The message that was sent by the other user</returns>
    public static string ReadCipher(Session session)
    {
        // Read the message object from the xml file
        Message message = Read(session.Other!, session.User);

        // Get public key of the user
        var publicKey = Session.LoadUserPublicKey(session.Other!);

        // Validate the message
        if (!Validate(message, publicKey))
            throw new InvalidOperationException("Invalid message: signature does not match");

        return message.Text;
    }
    
    /// <summary>
    /// Decrypt a message from the session
    /// </summary>
    /// <param cypther="cipher">The cipher to read the message from</param>
    /// <param name="session">The session to read the message from</param>
    /// <returns>The message that was sent by the other user</returns>
    public static string Decipher(string cipher, Session session)
    {
        // Get the symmetric key
        var key = session.ReceiveSymmetricKey();

        // Decrypt the message
        (string text, byte[] iv, _) = AesCipher.DecomposeMessage(cipher);
        return AesCipher.Decrypt(text, key, iv);
    }

    /// <summary>
    /// Write a message to the session
    /// </summary>
    /// <param name="text">The text of the message</param>
    /// <param name="session">The session to write the message to</param>
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

    /// <summary>
    /// Write a message to the session
    /// </summary>
    /// <param name="t">The text of the message</param>
    /// <param name="s">The signature of the message</param>
    /// <param name="session">The session to write the message to</param>
    public static void WriteMessage(string t, string s, Session session)
    {
        // Save the message object to an xml file
        Write(new Message { Text = t, Signature = s },
            session.User, session.Other!);
    }

    /// <summary>
    /// Get the cipher of a message
    /// </summary>
    /// <param name="text">The text of the message</param>
    /// <param name="session">The session to get the cipher for</param>
    /// <returns> The cipher of the message and save it to a file</returns>
    public static Message GetCipher(string text, Session session)
    {
        // Encrypt the message
        string cipherText = AesCipher.Encrypt(text, session.SymmetricKey, out byte[] iv);
        string composedMessage = AesCipher.ComposeMessage(cipherText, iv, new byte[8]);

        // Sign the message
        string signature = DigitalSignature.Sign(cipherText, session.GetPrivateKey());

        // Save the message object to an xml file
        return new Message { Text = composedMessage, Signature = signature };
    }

    /// <summary>
    /// Read a message from the database
    /// </summary>
    /// <param name="sender">The sender of the message</param>
    /// <param name="receiver">The receiver of the message</param>
    /// <returns>The message that was sent by the sender</returns>
    private static Message Read(UserDB sender, UserDB receiver)
    {
        // Read the message from the database
        var message = DatabaseController.GetMessage(sender, receiver);

        // Check if the message was found
        if (message is null)
            // Throw an exception if the message was not found
            throw new InvalidOperationException("Message not found");

        // Return the message
        Message m = new Message { Text = message.text, Signature = message.signature };

        return m;
    }

    /// <summary>
    /// Write a message to the database
    /// </summary>
    /// <param name="message">The message to write</param>
    /// <param name="sender">The sender of the message</param>
    /// <param name="receiver">The receiver of the message</param>
    private static void Write(Message message, UserDB sender, UserDB receiver)
    {
        // Check if message between sender an receiver exists
        var m = DatabaseController.GetMessage(sender, receiver);

        // Update the message if it exists
        if (m is not null)
        {
            m.text = message.Text;
            m.signature = message.Signature;
            DatabaseController.UpdateMessage(m);
        } // Otherwise add the message
        else DatabaseController.AddMessage(sender, receiver, message.Text, message.Signature);
    }

    /// <summary>
    /// Validate the message signature
    /// </summary>
    /// <param name="message">The message to validate</param>
    /// <param name="publicKey">The public key of the sender</param>
    /// <returns>True if the message is valid, false otherwise</returns>
    private static bool Validate(Message message, byte[] publicKey)
    {
        // Decompose the message
        (string text, _, _) = AesCipher.DecomposeMessage(message.Text);
        // Validate the message
        return DigitalSignature.Verify(text, message.Signature, publicKey);
    }
}

/// <summary>
/// This class is used to manage messages
/// </summary>
public class Message
{
    /// <summary>
    /// The text of the message
    /// </summary>
    public required string Text { get; set; }

    /// <summary>
    /// The signature of the message
    /// </summary>
    public required string Signature { get; set; }

}