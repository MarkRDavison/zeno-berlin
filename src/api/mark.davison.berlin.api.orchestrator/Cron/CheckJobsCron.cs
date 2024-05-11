namespace mark.davison.berlin.api.orchestrator.Cron;

public class CheckJobsCron : CronJobService
{
    private readonly IDistributedPubSub _distributedPubSubService;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<CheckJobsCron> _logger;
    private readonly AppSettings _appSettings;

    public CheckJobsCron(
        IScheduleConfig<CheckJobsCron> scheduleConfig,
        IDistributedPubSub distributedPubSubService,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<CheckJobsCron> logger,
        IOptions<AppSettings> appSettings) : base(
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

        var repository = scope.ServiceProvider.GetRequiredService<IRepository>();

        await using (repository.BeginTransaction())
        {
            var jobs = await repository.QueryEntities<Job>()
                .Where(
                    _ =>
                        _.Status != JobStatusConstants.Complete &&
                        _.Status != JobStatusConstants.Running &&
                        _.Status != JobStatusConstants.Errored)
                .CountAsync(cancellationToken);

            if (jobs == 0)
            {
                _logger.LogInformation("There are no jobs, not triggering a check");
                return;
            }

            await _distributedPubSubService.TriggerNotificationAsync(_appSettings.JOBS.JOB_CHECK_EVENT_KEY, cancellationToken);
        }
    }
}
