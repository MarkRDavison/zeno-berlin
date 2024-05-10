namespace mark.davison.berlin.api.orchestrator.Cron;

public class MonthlyNotificationsCron : BaseCQRSCron<MonthlyNotificationsCommandRequest, MonthlyNotificationsCommandResponse, MonthlyNotificationsCron>
{
    public MonthlyNotificationsCron(
        IScheduleConfig<MonthlyNotificationsCron> scheduleConfig,
        IDateService dateService,
        IDistributedPubSub distributedPubSubService,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<MonthlyNotificationsCron> logger,
        IOptions<AppSettings> appSettings
    ) : base(
        scheduleConfig,
        dateService,
        distributedPubSubService,
        serviceScopeFactory,
        logger,
        appSettings)
    {
    }
}
