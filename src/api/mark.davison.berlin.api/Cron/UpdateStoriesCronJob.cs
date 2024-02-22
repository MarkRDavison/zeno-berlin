namespace mark.davison.berlin.api.Cron;

public class UpdateStoriesCronJob : CronJobService
{
    private readonly ILogger<UpdateStoriesCronJob> _logger;
    private readonly IOptions<AppSettings> _appSettings;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public UpdateStoriesCronJob(
        string cronExpression,
        TimeZoneInfo timeZoneInfo,
        ILogger<UpdateStoriesCronJob> logger,
        IOptions<AppSettings> appSettings,
        IServiceScopeFactory serviceScopeFactory) : base(
        cronExpression,
        timeZoneInfo)
    {
        _logger = logger;
        _appSettings = appSettings;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public override async Task DoWork(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();

        var currentUserContext = scope.ServiceProvider.GetRequiredService<ICurrentUserContext>();
        var repo = scope.ServiceProvider.GetRequiredService<IReadonlyRepository>();

        await using (repo.BeginTransaction())
        {
            currentUserContext.CurrentUser = await repo.GetEntityAsync<User>(Guid.Empty, cancellationToken) ?? throw new InvalidOperationException("System user was not found");
        }

        var handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<UpdateStoriesRequest, UpdateStoriesResponse>>();

        _logger.LogInformation("Beginning update stories for {0} stories", _appSettings.Value.STORIES_PER_CRON_UPDATE);

        var request = new UpdateStoriesRequest { Amount = _appSettings.Value.STORIES_PER_CRON_UPDATE };
        var response = await handler.Handle(request, currentUserContext, cancellationToken);

        if (response.Success)
        {
            _logger.LogInformation("Successfully updated {0} stories.", _appSettings.Value.STORIES_PER_CRON_UPDATE);
        }
        else
        {
            _logger.LogError("Failed to update stories: {0}", string.Join(", ", response.Warnings.Concat(response.Errors)));
        }
    }
}
