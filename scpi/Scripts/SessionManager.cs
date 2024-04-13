namespace scpi;

/// <summary>
/// This class is used to manage sessions
/// </summary>
public class SessionManager
{
    /// <summary>
    /// The current session
    /// </summary>
    public Session? CurrentSession { get; private set; }

    /// <summary>
    /// Create a new session
    /// </summary>
    /// <param name="user">The username of the user</param>
    /// <param name="password">The password of the user</param>
    /// <param name="manager">The session manager that was created</param>
    /// <returns>True if the session was created successfully, false otherwise</returns>
    public static bool CreateSession(string user, string password, out SessionManager manager)
    {
        // Create a new session manager
        manager = new SessionManager();
        // Try to create a new session
        try
        {
            // Create a new session
            manager.CurrentSession = new Session(user, password)
            {
                // Set the generated flag to false
                Generated = false
            };
            // Return true if the session was created successfully
            return true;
        }
        catch (Exception e)
        {
            // Set the current session to null
            manager.CurrentSession = null;
            // Print the error message
            Console.WriteLine(e.Message);
            // Return false if the session could not be created
            return false;
        }
    }

    /// <summary>
    /// Generate the keys for the session user
    /// </summary>
    public void GenerateKeys()
    {
        // Generate the keys for the session user
        CurrentSession!.GenerateKeys();
        // Update the user in the database
        DatabaseController.UpdateUser(CurrentSession!.User);
        // Set the generated flag to true
        CurrentSession.Generated = true;
    }

    /// <summary>
    /// Share the symmetric key with the other user
    public void ShareSymmetricKey()
    {
        CurrentSession?.ShareSymmetricKey();
    }

    /// <summary>
    /// Set the other user for the session
    /// </summary>
    /// <returns>True if the other user was set successfully, false otherwise</returns>
    public bool SetOtherUser()
    {
        // Get all the users from the database
        var others = DatabaseController.GetAllUsers();
        // Return false if there are no other users
        if (others.Count <= 1)
            return false;
        // Get the other user from the list if it is not the current user
        var other = others.First(x => x.id != CurrentSession?.User.id);
        // Set the other user for the session
        CurrentSession?.SetOtherUser(other);
        return true;
    }

    /// <summary>
    /// Destroy the current session
    /// </summary>
    public void DestroySession()
    {
        CurrentSession = null;
    }

    /// <summary>
    /// Save the keys for the session user
    /// </summary>
    /// <returns>The private and public keys of the session user</returns>
    public (string, string) SaveKeys()
    {
        // Return the keys for the session user if the session is not null or throw an exception
        return CurrentSession?.SaveKeys() ??
            throw new InvalidOperationException("No session found");
    }

    /// <summary>
    /// Load the keys for the session user
    /// </summary>
    /// <param name="privKey">The private key of the session user</param>
    /// <param name="pubKey">The public key of the session user</param>
    public void LoadKeys(string privKey, string pubKey)
    {
        CurrentSession?.LoadKeys(privKey, pubKey);
        // Update the user in the database
        DatabaseController.UpdateUser(CurrentSession!.User);
    }

    /// <summary>
    /// Read a message from the database
    /// </summary>
    /// <returns>The message that was sent by the sender</returns>
    public string ReadMessage()
    {
        return MessageManager.ReadMessage(CurrentSession!);
    }

    /// <summary>
    /// Write a message to the database
    /// </summary>
    /// <param name="text">The text of the message</param>
    public void WriteMessage(string text)
    {
        MessageManager.WriteMessage(text, CurrentSession!);
    }

    /// <summary>
    /// Get the cipher of a message
    /// </summary>
    /// <param name="text">The text of the message</param>
    /// <returns> The cipher of the message and save it to a file</returns>
    public Message GetCipher(string text)
    {
        return MessageManager.GetCipher(text, CurrentSession!);
    }
}

/// <summary>
/// This class is used to save the sessions data
/// </summary>
public static class Sessions
{
    /// <summary>
    /// The list of sessions
    /// </summary>
    public static Dictionary<int, SessionManager> SessionsList = new();

    /// <summary>
    /// The session counter
    /// </summary>
    private static int sessionCounter = 0;

    /// <summary>
    /// Get a session by id
    /// </summary>
    public static SessionManager? GetSession(int id)
    {
        return SessionsList.ContainsKey(id) ? SessionsList[id] : null;
    }

    /// <summary>
    /// Add a session to the list
    /// </summary>
    public static int AddSession(SessionManager session)
    {
        sessionCounter++;
        SessionsList[sessionCounter] = session;
        return sessionCounter;
    }

    /// <summary>
    /// Remove a session from the list
    /// </summary>
    public static void RemoveSession(int id)
    {
        SessionsList.Remove(id);
    }
}