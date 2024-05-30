namespace mark.davison.berlin.api.orchestrator.Cron;

public abstract class BaseCQRSCron<TRequest, TResponse, TCron> : CronJobService
    where TRequest : class, ICommand<TRequest, TResponse>, new()
    where TResponse : Response, new()
{
    private readonly IDateService _dateService;
    private readonly IDistributedPubSub _distributedPubSubService;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    protected readonly ILogger _logger;
    protected readonly AppSettings _appSettings;

    protected BaseCQRSCron(
        IScheduleConfig<TCron> scheduleConfig,
        IDateService dateService,
        IDistributedPubSub distributedPubSubService,
        IServiceScopeFactory serviceScopeFactory,
        ILogger logger,
        IOptions<AppSettings> appSettings
    ) : base(
        scheduleConfig.CronExpression,
        scheduleConfig.TimeZoneInfo)
    {
        _dateService = dateService;
        _distributedPubSubService = distributedPubSubService;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _appSettings = appSettings.Value;
    }

    protected virtual TRequest CreateRequest() => new TRequest();

    protected virtual bool TriggerJobCheck => true;

    public override async Task DoWork(CancellationToken cancellationToken)
    {
        var request = CreateRequest();

        _logger.LogInformation("Creating scheduled '{0}' job request", typeof(TRequest).Name);

        using var scope = _serviceScopeFactory.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<IDbContext<BerlinDbContext>>();

        var job = CreateJob(request);

        await dbContext.AddAsync(job, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Scheduled '{0}' job request created", typeof(TRequest).Name);

        if (TriggerJobCheck)
        {
            await _distributedPubSubService.TriggerNotificationAsync(_appSettings.JOBS.JOB_CHECK_EVENT_KEY, cancellationToken);
        }
    }

    protected virtual Job CreateJob(TRequest request)
    {
        if (request is IJobRequest { UseJob: true })
        {
            _logger.LogWarning("'{0}' job is being submitted with UseJob = true, this means a job will create a job, consider setting this to false unless you really want to do this", typeof(TRequest).Name);
        }

        return new Job
        {
            Id = Guid.NewGuid(),
            ContextUserId = default,
            UserId = default,
            JobType = typeof(TRequest).AssemblyQualifiedName!,
            Created = _dateService.Now,
            JobRequest = JsonSerializer.Serialize(request),
            Status = JobStatusConstants.Submitted,
            SubmittedAt = _dateService.Now,
            LastModified = _dateService.Now
        };
    }
}
