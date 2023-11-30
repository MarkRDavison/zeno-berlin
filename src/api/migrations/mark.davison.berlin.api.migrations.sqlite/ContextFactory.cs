namespace mark.davison.berlin.api.migrations.sqlite;

public class ContextFactory : SqliteDbContextFactory<BerlinDbContext>
{
    protected override BerlinDbContext DbContextCreation(
            DbContextOptions<BerlinDbContext> options
        ) => new BerlinDbContext(options);
}
