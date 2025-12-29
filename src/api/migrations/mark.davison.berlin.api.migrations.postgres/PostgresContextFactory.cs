namespace mark.davison.berlin.api.migrations.postgres;

[ExcludeFromCodeCoverage]
[DatabaseMigrationAssembly(DatabaseType.Postgres)]
public sealed class PostgresContextFactory : PostgresDbContextFactory<BerlinDbContext>
{
    protected override string ConfigName => "DATABASE";

    protected override BerlinDbContext DbContextCreation(
            DbContextOptions<BerlinDbContext> options
        ) => new BerlinDbContext(options);
}
