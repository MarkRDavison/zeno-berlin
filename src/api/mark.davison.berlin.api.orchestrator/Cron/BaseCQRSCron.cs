﻿using mark.davison.berlin.shared.models.dtos;

namespace mark.davison.berlin.api.orchestrator.Cron;

public abstract class BaseCQRSCron<TRequest, TResponse, TCron> : CronJobService
    where TRequest : class, ICommand<TRequest, TResponse>, new()
    where TResponse : Response, new()
{
    private readonly IDateService _dateService;
    private readonly IRedisService _redisService;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger _logger;
    private readonly AppSettings _appSettings;

    protected BaseCQRSCron(
        IScheduleConfig<TCron> scheduleConfig,
        IDateService dateService,
        IRedisService redisService,
        IServiceScopeFactory serviceScopeFactory,
        ILogger logger,
        IOptions<AppSettings> appSettings
    ) : base(
        scheduleConfig.CronExpression,
        scheduleConfig.TimeZoneInfo)
    {
        _dateService = dateService;
        _redisService = redisService;
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

        var repository = scope.ServiceProvider.GetRequiredService<IRepository>();

        await using (repository.BeginTransaction())
        {
            var job = CreateJob(request);

            await repository.UpsertEntityAsync(job, cancellationToken);
        }

        _logger.LogInformation("Scheduled '{0}' job request created", typeof(TRequest).Name);

        if (TriggerJobCheck)
        {
            await _redisService.SetValueAsync(_appSettings.JOB_CHECK_EVENT_KEY_NAME, "CHECK", TimeSpan.FromSeconds(10), cancellationToken);
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