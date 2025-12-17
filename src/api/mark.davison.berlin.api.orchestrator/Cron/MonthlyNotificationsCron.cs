namespace mark.davison.berlin.api.orchestrator.Cron;

public class MonthlyNotificationsCron(
    IScheduleConfig<MonthlyNotificationsCron> scheduleConfig,
    IDateService dateService,
    IDistributedPubSub distributedPubSubService,
    IServiceScopeFactory serviceScopeFactory,
    ILogger<MonthlyNotificationsCron> logger,
    IOptions<OrchestratorAppSettings> appSettings
) : BaseCQRSCron<MonthlyNotificationsCommandRequest, MonthlyNotificationsCommandResponse, MonthlyNotificationsCron>(
    scheduleConfig,
    dateService,
    distributedPubSubService,
    serviceScopeFactory,
    logger,
    appSettings)
{
}