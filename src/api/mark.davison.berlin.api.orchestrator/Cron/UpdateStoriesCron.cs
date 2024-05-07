namespace mark.davison.berlin.api.orchestrator.Cron;

public class UpdateStoriesCron : BaseCQRSCron<UpdateStoriesRequest, UpdateStoriesResponse, UpdateStoriesCron>
{
    public UpdateStoriesCron(
        IScheduleConfig<UpdateStoriesCron> scheduleConfig,
        IDateService dateService,
        IRedisService redisService,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<UpdateStoriesCron> logger,
        IOptions<AppSettings> appSettings
    ) : base(
        scheduleConfig,
        dateService,
        redisService,
        serviceScopeFactory,
        logger,
        appSettings)
    {
    }
}
