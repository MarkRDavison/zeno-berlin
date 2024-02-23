namespace mark.davison.berlin.api.Cron;

public class UpdateStoriesCronJob : CQRSCronJob<UpdateStoriesCronJob, UpdateStoriesRequest, UpdateStoriesResponse>
{
    private readonly ILogger<UpdateStoriesCronJob> _logger;
    private readonly IOptions<AppSettings> _appSettings;

    public UpdateStoriesCronJob(
        ILogger<UpdateStoriesCronJob> logger,
        IOptions<AppSettings> appSettings,
        IScheduleConfig<UpdateStoriesCronJob> scheduleConfig,
        IServiceScopeFactory serviceScopeFactory
    ) : base(
        scheduleConfig,
        serviceScopeFactory)
    {
        _logger = logger;
        _appSettings = appSettings;
    }

    protected override Guid JobUserId => Guid.Empty;

    protected override UpdateStoriesRequest CreateRequest()
    {
        _logger.LogInformation("Beginning update stories for {0} stories", _appSettings.Value.STORIES_PER_CRON_UPDATE);
        return new UpdateStoriesRequest { Amount = _appSettings.Value.STORIES_PER_CRON_UPDATE };
    }

    protected override Task HandleResponse(UpdateStoriesResponse response)
    {
        if (response.Success)
        {
            _logger.LogInformation("Successfully updated {0} stories.", _appSettings.Value.STORIES_PER_CRON_UPDATE);
        }
        else
        {
            _logger.LogError("Failed to update stories: {0}", string.Join(", ", response.Warnings.Concat(response.Errors)));
        }

        return Task.CompletedTask;
    }
}
