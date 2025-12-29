namespace mark.davison.berlin.api.jobs;


public class ApplicationHealthStateHostedService : ApiApplicationHealthStateHostedService<BerlinDbContext, JobsAppSettings>
{

    private readonly IJobOrchestrationService _jobOrchestrationService;

    public ApplicationHealthStateHostedService(
        IApplicationHealthState applicationHealthState,
        IHostApplicationLifetime hostApplicationLifetime,
        IDbContextFactory<BerlinDbContext> dbContextFactory,
        IOptions<JobsAppSettings> appSettings,
        IJobOrchestrationService jobOrchestrationService) : base(
        applicationHealthState,
        hostApplicationLifetime,
        dbContextFactory,
        appSettings,
        null)
    {
        _jobOrchestrationService = jobOrchestrationService;
    }

    protected override async Task AdditionalStartAsync(CancellationToken cancellationToken)
    {
        await _jobOrchestrationService.InitialiseJobMonitoring();
    }

    protected override async Task InitDatabaseProduction(BerlinDbContext dbContext, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }

    protected override async Task InitDatabaseDevelopment(BerlinDbContext dbContext, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}
