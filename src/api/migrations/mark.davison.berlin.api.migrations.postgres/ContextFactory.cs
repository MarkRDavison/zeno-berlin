namespace mark.davison.berlin.api.migrations.postgres;

[ExcludeFromCodeCoverage]
public sealed class ContextFactory : PostgresDbContextFactory<BerlinDbContext>
{
    protected override string ConfigName => "DATABASE";

    protected override BerlinDbContext DbContextCreation(
            DbContextOptions<BerlinDbContext> options
        ) => new BerlinDbContext(options);
}
