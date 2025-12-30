namespace mark.davison.berlin.api.orchestrator;

public class ApplicationHealthStateHostedService(
    IApplicationHealthState applicationHealthState,
    IHostApplicationLifetime hostApplicationLifetime,
    IDbContextFactory<BerlinDbContext> dbContextFactory,
    IOptions<OrchestratorAppSettings> appSettings
) : ApiApplicationHealthStateHostedService<BerlinDbContext, OrchestratorAppSettings>(
    applicationHealthState,
    hostApplicationLifetime,
    dbContextFactory,
    appSettings,
    null)
{
    protected override async Task InitDatabaseProduction(BerlinDbContext dbContext, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }

    protected override async Task InitDatabaseDevelopment(BerlinDbContext dbContext, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}
