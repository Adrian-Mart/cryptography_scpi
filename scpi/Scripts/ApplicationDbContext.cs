using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace scpi;

/// <summary>
/// This class is the database context for the application
/// </summary>
public class ApplicationDbContext : DbContext
{
    /// <summary>
    /// Constructor for the database context
    /// </summary>
    /// <param name="options">The options for the database context</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    { }

    /// <summary>
    /// This method is called when the model is created
    /// </summary>
    /// <param name="modelBuilder">The model builder</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Set the primary key for the messages table
        modelBuilder.Entity<MessageDB>().HasKey(s => new { s.sender_id, s.receiver_id });
        // Set the primary key for the shared_keys table
        modelBuilder.Entity<SharedKeyDB>().HasKey(s => new { s.sender_id, s.receiver_id });
    }

    /// <summary>
    /// The users table
    /// </summary>
    public DbSet<UserDB> users { get; set; }

    /// <summary>
    /// The messages table
    /// </summary>
    public DbSet<MessageDB> messages { get; set; }

    /// <summary>
    /// The shared_keys table
    /// </summary>
    public DbSet<SharedKeyDB> shared_keys { get; set; }
}

/// <summary>
/// This class represents a user in the database
/// </summary>
public class UserDB
{
    /// <summary>
    /// The user's ID
    /// </summary>
    public int id { get; set; }

    /// <summary>
    /// The user's username
    /// </summary>
    public required string user { get; set; }

    /// <summary>
    /// The user's password
    /// </summary>
    public required string password { get; set; }

    /// <summary>
    /// The user's public key
    /// </summary>
    public string? public_key { get; set; }
}

/// <summary>
/// This class represents a shared key in the database
/// </summary>
public class SharedKeyDB
{
    /// <summary>
    /// The ID of the sender
    /// </summary>
    public required int sender_id { get; set; }

    /// <summary>
    /// The ID of the receiver
    /// </summary>
    public required int receiver_id { get; set; }

    /// <summary>
    /// The shared key
    /// </summary>
    public required string key { get; set; }
}

/// <summary>
/// This class represents a message in the database
/// </summary>
public class MessageDB
{
    /// <summary>
    /// The ID of the sender
    /// </summary>
    public required int sender_id { get; set; }

    /// <summary>
    /// The ID of the receiver
    /// </summary>
    public required int receiver_id { get; set; }

    /// <summary>
    /// The text of the message
    /// </summary>
    public required string text { get; set; }

    /// <summary>
    /// The signature of the message
    /// </summary>
    public required string signature { get; set; }
}