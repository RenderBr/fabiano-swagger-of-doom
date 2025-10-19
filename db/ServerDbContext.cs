using db.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace db;

public class ServerDbContext : DbContext
{
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Character> Characters => Set<Character>();
    public DbSet<GuildEntity> Guilds => Set<GuildEntity>();
    public DbSet<Death> Deaths => Set<Death>();
    public DbSet<ClassStat> ClassStats => Set<ClassStat>();
    public DbSet<Vault> Vaults => Set<Vault>();
    public DbSet<DailyQuest> DailyQuests => Set<DailyQuest>();
    public DbSet<Pet> Pets => Set<Pet>();
    public DbSet<Stat> Stats => Set<Stat>();
    public DbSet<News> News => Set<News>();
    public DbSet<GlobalNews> GlobalNews => Set<GlobalNews>();
    public DbSet<Board> Boards => Set<Board>();
    public DbSet<ArenaLeaderboard> ArenaLeaderboards => Set<ArenaLeaderboard>();
    public DbSet<Backpack> Backpacks => Set<Backpack>();
    public DbSet<GiftCode> GiftCodes => Set<GiftCode>();
    public DbSet<Package> Packages => Set<Package>();
    public DbSet<MysteryBox> MysteryBoxes => Set<MysteryBox>();
    public DbSet<ClientError> ClientErrors => Set<ClientError>();
    public DbSet<UnlockedClass> UnlockedClasses => Set<UnlockedClass>();
    private readonly ILogger<ServerDbContext> _logger;
    
    public ServerDbContext(DbContextOptions<ServerDbContext> options, ILogger<ServerDbContext> logger)
        : base(options)
    {
        _logger = logger;
        _logger.LogInformation("ServerDbContext initialized.");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _logger.LogInformation("Configuring model relationships and keys.");
        base.OnModelCreating(modelBuilder);

        // Configure composite keys
        modelBuilder.Entity<ClassStat>()
            .HasKey(cs => new { cs.AccountId, cs.ObjectType });

        modelBuilder.Entity<Backpack>()
            .HasKey(b => new { b.AccountId, b.CharacterId });

        // Configure relationships
        modelBuilder.Entity<Character>()
            .HasOne(c => c.Account)
            .WithMany()
            .HasForeignKey(c => c.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Death>()
            .HasOne(d => d.Account)
            .WithMany()
            .HasForeignKey(d => d.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ClassStat>()
            .HasOne(cs => cs.Account)
            .WithMany()
            .HasForeignKey(cs => cs.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Vault>()
            .HasOne(v => v.Account)
            .WithMany(a => a.Vaults)
            .HasForeignKey(v => v.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DailyQuest>()
            .HasOne(dq => dq.Account)
            .WithMany()
            .HasForeignKey(dq => dq.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Pet>()
            .HasOne(p => p.Account)
            .WithMany()
            .HasForeignKey(p => p.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Stat>()
            .HasOne(s => s.Account)
            .WithMany()
            .HasForeignKey(s => s.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ArenaLeaderboard>()
            .HasOne(a => a.Account)
            .WithMany()
            .HasForeignKey(a => a.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Backpack>()
            .HasOne(b => b.Account)
            .WithMany()
            .HasForeignKey(b => b.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Board>()
            .HasOne(b => b.Guild)
            .WithOne(g => g.Board)
            .HasForeignKey<Board>(b => b.GuildId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Account>()
            .HasOne(a => a.Guild)
            .WithMany(g => g.Accounts)
            .HasForeignKey(a => a.GuildId)
            .HasPrincipalKey(g => g.Id)
            .OnDelete(DeleteBehavior.Restrict);
        
        _logger.LogInformation("Model configuration complete.");
    }
}