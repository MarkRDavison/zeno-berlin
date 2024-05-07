namespace mark.davison.berlin.api.orchestrator.Cron;

public class MonthlyNotificationsCron : BaseCQRSCron<MonthlyNotificationsCommandRequest, MonthlyNotificationsCommandResponse, MonthlyNotificationsCron>
{
    public MonthlyNotificationsCron(
        IScheduleConfig<MonthlyNotificationsCron> scheduleConfig,
        IDateService dateService,
        IRedisService redisService,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<MonthlyNotificationsCron> logger,
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
