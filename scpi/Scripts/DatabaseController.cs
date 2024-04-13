using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace scpi;

public static class DatabaseController
{
    private static bool isActive;
    private static ApplicationDbContext? context;
    private static int userId = 0;


    public static void Initialize(ApplicationDbContext context)
    {
        isActive = true;
        DatabaseController.context = context;
        userId = context.users.Count() > 0 ? context.users.Max(u => u.id) : 0;
    }

    public static void AddUser(string username, string password, string publicKey)
    {
        if (!isActive) throw new Exception("Database is not active");
        userId++;
        var hashvalue = SHA3_256.HashData(System.Text.Encoding.UTF8.GetBytes(password));
        context!.users.Add(new UserDB
        {
            id = userId,
            user = username,
            password = Convert.ToBase64String(hashvalue),
            public_key = publicKey
        });
        context.SaveChanges();
    }

    public static void AddSharedKey(UserDB sender, UserDB receiver, string key)
    {
        if (!isActive) throw new Exception("Database is not active");
        context!.shared_keys.Add(new SharedKeyDB
        {
            sender_id = sender.id,
            receiver_id = receiver.id,
            key = key
        });
        context.SaveChanges();
    }

    public static void AddMessage(UserDB sender, UserDB receiver, string text, string signature)
    {
        if (!isActive) throw new Exception("Database is not active");
        context!.messages.Add(new MessageDB
        {
            sender_id = sender.id,
            receiver_id = receiver.id,
            text = text,
            signature = signature
        });
        context.SaveChanges();
    }

    public static UserDB? GetUser(string username)
    {
        if (!isActive) throw new Exception("Database is not active");
        return context!.users.FirstOrDefault(u => u.user == username);
    }

    public static SharedKeyDB? GetSharedKey(UserDB sender, UserDB receiver)
    {
        if (!isActive) throw new Exception("Database is not active");
        try
        {
            return context!.shared_keys.First(k => k.sender_id == sender.id && k.receiver_id == receiver.id);
        }
        catch
        {
            return null;
        }
    }

    public static MessageDB? GetMessage(UserDB sender, UserDB receiver)
    {
        if (!isActive) throw new Exception("Database is not active");
        return context!.messages.FirstOrDefault(m => m.sender_id == sender.id && m.receiver_id == receiver.id);
    }

    public static void UpdateUser(UserDB userDB)
    {
        if (!isActive) throw new Exception("Database is not active");

        // Get the existing user from the database
        var existingUser = context!.users.Find(userDB.id);

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

    public static void UpdateSharedKey(SharedKeyDB keyDB)
    {
        if (!isActive) throw new Exception("Database is not active");
        context!.shared_keys.Update(keyDB);
        context.SaveChanges();
    }

    public static void UpdateMessage(MessageDB message)
    {
        if (!isActive) throw new Exception("Database is not active");
        context!.messages.Update(message);
        context.SaveChanges();
    }

    public static List<UserDB> GetAllUsers()
    {
        if (!isActive) throw new Exception("Database is not active");
        return context!.users.ToList();
    }
}