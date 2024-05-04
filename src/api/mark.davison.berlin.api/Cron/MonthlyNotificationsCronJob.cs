namespace mark.davison.berlin.api.Cron;

public class MonthlyNotificationsCronJob : CQRSCronJob<MonthlyNotificationsCronJob, MonthlyNotificationsCommandRequest, MonthlyNotificationsCommandResponse>
{
    private readonly ILogger<MonthlyNotificationsCronJob> _logger;

    public MonthlyNotificationsCronJob(
        IScheduleConfig<MonthlyNotificationsCronJob> scheduleConfig,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<MonthlyNotificationsCronJob> logger
    ) : base(
        scheduleConfig,
        serviceScopeFactory)
    {
        _logger = logger;
    }

    protected override Guid JobUserId => Guid.Empty;

    protected override MonthlyNotificationsCommandRequest CreateRequest()
    {
        return new MonthlyNotificationsCommandRequest { };
    }

    protected override Task HandleResponse(MonthlyNotificationsCommandResponse response)
    {
        if (response.Success)
        {
            if (response.Warnings.Any(_ => _ == ValidationMessages.NO_ITEMS))
            {
                _logger.LogInformation("No stories were found that required monthly notifications.");
            }
            else
            {
                _logger.LogInformation(
                    "Successfully sent notifications for {0} monthly stories",
                    response.StoriesNotified);
            }
        }
        else
        {
            _logger.LogError("Failed to notify monthly stories: {0}", string.Join(", ", response.Warnings.Concat(response.Errors)));
        }

        return Task.CompletedTask;
    }
}
