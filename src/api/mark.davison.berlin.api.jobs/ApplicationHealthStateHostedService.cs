namespace mark.davison.berlin.api.jobs;

public class ApplicationHealthStateHostedService(
    IApplicationHealthState applicationHealthState,
    IHostApplicationLifetime hostApplicationLifetime,
    IDbContextFactory<BerlinDbContext> dbContextFactory,
    IOptions<JobsAppSettings> appSettings
) : ApiApplicationHealthStateHostedService<BerlinDbContext, JobsAppSettings>(
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
