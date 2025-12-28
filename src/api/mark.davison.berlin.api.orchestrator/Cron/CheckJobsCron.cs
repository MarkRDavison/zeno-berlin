namespace mark.davison.berlin.api.orchestrator.Cron;

public class CheckJobsCron : CronJobService
{
    private readonly IDistributedPubSub _distributedPubSubService;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<CheckJobsCron> _logger;
    private readonly OrchestratorAppSettings _appSettings;

    public CheckJobsCron(
        IScheduleConfig<CheckJobsCron> scheduleConfig,
        IDistributedPubSub distributedPubSubService,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<CheckJobsCron> logger,
        IOptions<OrchestratorAppSettings> appSettings) : base(
        scheduleConfig.CronExpression,
        scheduleConfig.TimeZoneInfo)
    {
        _distributedPubSubService = distributedPubSubService;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _appSettings = appSettings.Value;
    }

    public override async Task DoWork(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<IDbContext<BerlinDbContext>>();

        var jobs = await dbContext
            .Set<Job>()
            .Where(_ =>
                _.Status != JobStatusConstants.Complete &&
                _.Status != JobStatusConstants.Running &&
                _.Status != JobStatusConstants.Errored)
            .CountAsync(cancellationToken);

        if (jobs == 0)
        {
            return;
        }

        _logger.LogInformation("Found {0} jobs, triggering a check", jobs);

        await _distributedPubSubService.TriggerNotificationAsync(_appSettings.JOBS.JOB_CHECK_EVENT_KEY, cancellationToken);
    }
}