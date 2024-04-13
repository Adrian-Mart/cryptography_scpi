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
        modelBuilder.Entity<MessageDB>().HasKey(s => new { s.sender_id, s.receiver_id });
        modelBuilder.Entity<SharedKeyDB>().HasKey(s => new { s.sender_id, s.receiver_id });
    }

    public DbSet<UserDB> users { get; set; }

    public DbSet<MessageDB> messages { get; set; }

    public DbSet<SharedKeyDB> shared_keys { get; set; }
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
    public required int sender_id { get; set; }
    public required int receiver_id { get; set; }
    public required string key { get; set; }
}

public class MessageDB
{
    public required int sender_id { get; set; }
    public required int receiver_id { get; set; }
    public required string text { get; set; }
    public required string signature { get; set; }
}