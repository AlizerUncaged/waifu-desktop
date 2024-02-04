using Microsoft.EntityFrameworkCore;
using Serilog;
using Waifu.Models;

namespace Waifu.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<ChatChannel> ChatChannels { get; set; }

    public DbSet<ChatMessage> ChatMessages { get; set; }

    public DbSet<RoleplayCharacter> RoleplayCharacters { get; set; }

    public DbSet<Waifu.Models.Settings> Settings { get; set; }

    public DbSet<PersonaSingle> Personas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Configure SQLite as the database provider
        optionsBuilder.UseSqlite($"Data Source={Constants.DatabasePath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}