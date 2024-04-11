namespace scpi;

public class SessionManager
{
    public Session? CurrentSession { get; private set; }

    public static bool CreateSession(string user, string password, out SessionManager manager)
    {
        manager = new SessionManager();
        try
        {
            manager.CurrentSession = new Session(user, password);
            return true;
        }
        catch (Exception e)
        {
            manager.CurrentSession = null;
            Console.WriteLine(e.Message);
            return false;
        }
    }

    public void GenerateKeys()
    {
        CurrentSession?.GenerateKeys();
    }

    public void ShareSymmetricKey()
    {
        CurrentSession?.ShareSymmetricKey();
    }

    public bool SetOtherUser()
    {
        var others = DatabaseController.GetAllUsers();
        if (others.Count <= 1)
            return false;
        var other = others.First(x => x.id != CurrentSession?.User.id);
        CurrentSession?.SetOtherUser(other);
        return true;
    }

    public void DestroySession()
    {
        CurrentSession = null;
    }

    public (string, string) SaveKeys()
    {
        return CurrentSession?.SaveKeys() ??
            throw new InvalidOperationException("No session found");
    }

    public void LoadKeys(string privKey)
    {
        CurrentSession?.LoadKeys(privKey);
    }

    public string ReadMessage()
    {
        return MessageManager.ReadMessage(CurrentSession!);
    }

    public void WriteMessage(string text)
    {
        MessageManager.WriteMessage(text, CurrentSession!);
    }

    public Message GetCipher(string text)
    {
        return MessageManager.GetCipher(text, CurrentSession!);
    }
}

public static class Sessions
{
    public static Dictionary<int, SessionManager> SessionsList = new();
    private static int sessionCounter = 0;

    public static SessionManager? GetSession(int id)
    {
        return SessionsList.ContainsKey(id) ? SessionsList[id] : null;
    }

    public static int AddSession(SessionManager session)
    {
        sessionCounter++;
        SessionsList[sessionCounter] = session;
        return sessionCounter;
    }

    public static void RemoveSession(int id)
    {
        SessionsList.Remove(id);
    }
}