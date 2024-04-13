using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace scpi;

/// <summary>
/// This class is used to interact with the database
/// </summary>
public static class DatabaseController
{
    /// <summary>
    /// Whether the database is active
    /// </summary>
    private static bool isActive;

    /// <summary>
    /// The database context
    /// </summary>
    private static ApplicationDbContext? context;

    /// <summary>
    /// The user ID
    /// </summary>
    private static int userId = 0;

    /// <summary>
    /// Initializes the database controller
    /// </summary>
    /// <param name="context">The database context</param>
    public static void Initialize(ApplicationDbContext context)
    {
        // Set the database to active
        isActive = true;
        // Set the database context
        DatabaseController.context = context;
        // Get the highest user ID from the database or set it to 0 if there are no users
        userId = context.users.Count() > 0 ? context.users.Max(u => u.id) : 0;
    }

    /// <summary>
    /// Adds a user to the database
    /// </summary>
    /// <param name="username">The username of the user</param>
    /// <param name="password">The password of the user</param>
    /// <param name="publicKey">The public key of the user</param>
    public static void AddUser(string username, string password, string publicKey)
    {
        // If the database is not active, throw an exception
        if (!isActive) throw new Exception("Database is not active");

        // Increment the user ID
        userId++;

        // Hash the password using SHA3-256
        var hashvalue = SHA3_256.HashData(System.Text.Encoding.UTF8.GetBytes(password));

        // Add the user to the database and save the changes
        context!.users.Add(new UserDB
        {
            id = userId,
            user = username,
            password = Convert.ToBase64String(hashvalue),
            public_key = publicKey
        });
        context.SaveChanges();
    }
    
    /// <summary>
    /// Adds a shared key to the database
    /// </summary>
    /// <param name="sender">The sender of the shared key</param>
    /// <param name="receiver">The receiver of the shared key</param>
    /// <param name="key">The shared key</param>
    public static void AddSharedKey(UserDB sender, UserDB receiver, string key)
    {
        // If the database is not active, throw an exception
        if (!isActive) throw new Exception("Database is not active");

        // Add the shared key to the database and save the changes
        context!.shared_keys.Add(new SharedKeyDB
        {
            sender_id = sender.id,
            receiver_id = receiver.id,
            key = key
        });
        context.SaveChanges();
    }

    /// <summary>
    /// Adds a message to the database
    /// </summary>
    /// <param name="sender">The sender of the message</param>
    /// <param name="receiver">The receiver of the message</param>
    /// <param name="text">The text of the message</param>
    /// <param name="signature">The signature of the message</param>
    public static void AddMessage(UserDB sender, UserDB receiver, string text, string signature)
    {
        // If the database is not active, throw an exception
        if (!isActive) throw new Exception("Database is not active");

        // Add the message to the database and save the changes
        context!.messages.Add(new MessageDB
        {
            sender_id = sender.id,
            receiver_id = receiver.id,
            text = text,
            signature = signature
        });
        context.SaveChanges();
    }

    /// <summary>
    /// Gets a user from the database
    /// </summary>
    /// <param name="username">The username of the user</param>
    public static UserDB? GetUser(string username)
    {
        // If the database is not active, throw an exception
        if (!isActive) throw new Exception("Database is not active");

        // Get the user from the database and return it if the username matches
        return context!.users.FirstOrDefault(u => u.user == username);
    }

    /// <summary>
    /// Gets a user from the database
    /// </summary>
    /// <param name="id">The ID of the user</param>
    public static SharedKeyDB? GetSharedKey(UserDB sender, UserDB receiver)
    {
        // If the database is not active, throw an exception
        if (!isActive) throw new Exception("Database is not active");

        // Try to get the shared key from the database
        try
        {
            // Get the shared key from the database and return it if the sender and receiver match
            return context!.shared_keys.First(k => k.sender_id == sender.id && k.receiver_id == receiver.id);
        }
        catch
        {
            // If the shared key doesn't exist, return null
            return null;
        }
    }

    /// <summary>
    /// Gets a message from the database
    /// </summary>
    /// <param name="sender">The sender of the message</param>
    /// <param name="receiver">The receiver of the message</param>
    public static MessageDB? GetMessage(UserDB sender, UserDB receiver)
    {
        // If the database is not active, throw an exception
        if (!isActive) throw new Exception("Database is not active");

        // Get the message from the database and return it if the sender and receiver match
        return context!.messages.FirstOrDefault(m => m.sender_id == sender.id && m.receiver_id == receiver.id);
    }
    
    /// <summary>
    /// Updates a user in the database
    /// </summary>
    /// <param name="userDB">The user to update</param>
    public static void UpdateUser(UserDB userDB)
    {
        // If the database is not active, throw an exception
        if (!isActive) throw new Exception("Database is not active");

        // Get the existing user from the database
        var existingUser = context!.users.Find(userDB.id);

        // If the user doesn't exist, add it; otherwise, update it
        if (existingUser == null)
        {
            // The user doesn't exist, so add it
            context!.users.Add(userDB);
        }
        else
        {
            // The user does exist, so update it
            context.Entry(existingUser).CurrentValues.SetValues(userDB);
        }

        context.SaveChanges();
    }

    /// <summary>
    /// Updates a shared key in the database
    /// </summary>
    /// <param name="keyDB">The shared key to update</param>
    public static void UpdateSharedKey(SharedKeyDB keyDB)
    {
        // If the database is not active, throw an exception
        if (!isActive) throw new Exception("Database is not active");

        // Update the shared key in the database
        context!.shared_keys.Update(keyDB);
        context.SaveChanges();
    }

    /// <summary>
    /// Updates a message in the database
    /// </summary>
    /// <param name="message">The message to update</param>
    public static void UpdateMessage(MessageDB message)
    {
        // If the database is not active, throw an exception
        if (!isActive) throw new Exception("Database is not active");

        // Update the message in the database
        context!.messages.Update(message);
        context.SaveChanges();
    }

    /// <summary>
    /// Gets all users from the database
    /// </summary>
    /// <returns>A list of all users</returns>
    public static List<UserDB> GetAllUsers()
    {
        // If the database is not active, throw an exception
        if (!isActive) throw new Exception("Database is not active");

        // Get all users from the database and return them
        return context!.users.ToList();
    }
}