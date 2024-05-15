namespace mark.davison.berlin.api.migrations.sqlite;

[ExcludeFromCodeCoverage]
[DatabaseMigrationAssembly(DatabaseType.Sqlite)]
public sealed class SqliteContextFactory : SqliteDbContextFactory<BerlinDbContext>
{
    protected override BerlinDbContext DbContextCreation(
            DbContextOptions<BerlinDbContext> options
        ) => new BerlinDbContext(options);
}
