using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace scpi;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MessageDB>().HasNoKey();
        modelBuilder.Entity<SharedKeyDB>().HasNoKey();
    }

    public DbSet<UserDB> users { get; set; }

    public DbSet<MessageDB> messages { get; set; }

    public DbSet<SharedKeyDB> keys { get; set; }
}

public class UserDB
{
    public int id { get; set; }
    public required string user { get; set; }
    public required string password { get; set; }
    public string? public_key { get; set; }
}

public class SharedKeyDB
{
    public required int Sender_id { get; set; }
    public required int Receiver_id { get; set; }
    public required string Key { get; set; }
}

public class MessageDB
{
    public required int Sender_id { get; set; }
    public required int Receiver_id { get; set; }
    public required string Text { get; set; }
    public required string Signature { get; set; }
}