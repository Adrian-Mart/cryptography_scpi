using Microsoft.EntityFrameworkCore;
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
        userId = context.users.Max(u => u.id);
    }

    public static void AddUser(string username, string password, string publicKey)
    {
        if(!isActive) throw new Exception("Database is not active");
        userId++;
        context!.users.Add(new UserDB { id = userId,
            user = username, password = password, public_key = publicKey });
        context.SaveChanges();
    }

    public static void AddSharedKey(UserDB sender, UserDB receiver, string key)
    {
        if(!isActive) throw new Exception("Database is not active");
        context!.keys.Add(new SharedKeyDB { 
            Sender_id = sender.id, Receiver_id = receiver.id, Key = key });
        context.SaveChanges();
    }

    public static void AddMessage(UserDB sender, UserDB receiver, string text, string signature)
    {
        if(!isActive) throw new Exception("Database is not active");
        context!.messages.Add(new MessageDB { 
            Sender_id = sender.id, Receiver_id = receiver.id, Text = text, Signature = signature });
        context.SaveChanges();
    }

    public static UserDB? GetUser(string username)
    {
        if(!isActive) throw new Exception("Database is not active");
        return context!.users.FirstOrDefault(u => u.user == username);
    }

    public static SharedKeyDB? GetSharedKey(UserDB sender, UserDB receiver)
    {
        if(!isActive) throw new Exception("Database is not active");
        return context!.keys.FirstOrDefault(k => k.Sender_id == sender.id && k.Receiver_id == receiver.id);
    }

    public static MessageDB? GetMessage(UserDB sender, UserDB receiver)
    {
        if(!isActive) throw new Exception("Database is not active");
        return context!.messages.FirstOrDefault(m => m.Sender_id == sender.id && m.Receiver_id == receiver.id);
    }

    public static void UpdateUser(UserDB userDB)
    {
        if(!isActive) throw new Exception("Database is not active");
        context!.users.Update(userDB);
        context.SaveChanges();
    }

    public static void UpdateSharedKey(SharedKeyDB keyDB)
    {
        if(!isActive) throw new Exception("Database is not active");
        context!.keys.Update(keyDB);
        context.SaveChanges();
    }

    public static void UpdateMessage(MessageDB message)
    {
        if(!isActive) throw new Exception("Database is not active");
        context!.messages.Update(message);
        context.SaveChanges();
    }

    public static List<UserDB> GetAllUsers()
    {
        if(!isActive) throw new Exception("Database is not active");
        return context!.users.ToList();
    }
}