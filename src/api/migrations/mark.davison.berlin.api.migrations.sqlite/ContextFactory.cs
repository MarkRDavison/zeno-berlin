namespace mark.davison.berlin.api.migrations.sqlite;

[ExcludeFromCodeCoverage]
public class ContextFactory : SqliteDbContextFactory<BerlinDbContext>
{
    protected override BerlinDbContext DbContextCreation(
            DbContextOptions<BerlinDbContext> options
        ) => new BerlinDbContext(options);
}
