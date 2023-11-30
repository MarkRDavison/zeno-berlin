namespace mark.davison.berlin.api.persistence;

public class BerlinDbContext : DbContext
{
    public BerlinDbContext(DbContextOptions options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserEntityConfiguration).Assembly);
    }

    public DbSet<Document> Documents => Set<Document>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Share> Shares => Set<Share>();
    public DbSet<SharingOptions> SharingOptions => Set<SharingOptions>();
    public DbSet<StorageOptions> StorageOptions => Set<StorageOptions>();
    public DbSet<UserOptions> UserOptions => Set<UserOptions>();
}
