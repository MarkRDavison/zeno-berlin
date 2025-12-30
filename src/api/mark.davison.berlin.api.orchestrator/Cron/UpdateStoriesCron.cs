namespace mark.davison.berlin.api.orchestrator.Cron;

public sealed class UpdateStoriesCron(
    IScheduleConfig<UpdateStoriesCron> scheduleConfig,
    IDateService dateService,
    IDistributedPubSub distributedPubSubService,
    IServiceScopeFactory serviceScopeFactory,
    ILogger<UpdateStoriesCron> logger,
    IOptions<OrchestratorAppSettings> appSettings
) : BaseCQRSCron<UpdateStoriesRequest, UpdateStoriesResponse, UpdateStoriesCron>(
    scheduleConfig,
    dateService,
    distributedPubSubService,
    serviceScopeFactory,
    logger,
    appSettings)
{
    protected override UpdateStoriesRequest CreateRequest()
    {
        _logger.LogInformation("Creating job for updating stories");
        return new UpdateStoriesRequest
        {
            Amount = _appSettings.STORIES_PER_CRON_UPDATE
        };
    }
}