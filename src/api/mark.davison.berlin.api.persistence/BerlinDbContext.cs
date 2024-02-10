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

}
