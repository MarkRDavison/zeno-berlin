namespace mark.davison.berlin.api;

public class ApplicationHealthStateHostedService(
    IApplicationHealthState applicationHealthState,
    IHostApplicationLifetime hostApplicationLifetime,
    IDbContextFactory<BerlinDbContext> dbContextFactory,
    IOptions<ApiAppSettings> appSettings,
    IDataSeeder? dataSeeder
) : ApiApplicationHealthStateHostedService<BerlinDbContext, ApiAppSettings>(
    applicationHealthState,
    hostApplicationLifetime,
    dbContextFactory,
    appSettings,
    dataSeeder)
{
    protected override async Task InitDatabaseProduction(BerlinDbContext dbContext, CancellationToken cancellationToken)
    {
        if (PendingModelChangesChecker.HasPendingModelChanges(dbContext))
        {
            var details = PendingModelChangesChecker.GetPendingModelChanges(dbContext);
            Console.Error.WriteLine(details);
        }

        await dbContext.Database.MigrateAsync(cancellationToken);
    }

    protected override async Task InitDatabaseDevelopment(BerlinDbContext dbContext, CancellationToken cancellationToken)
    {
        await dbContext.Database.EnsureDeletedAsync(cancellationToken);
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);
    }
}