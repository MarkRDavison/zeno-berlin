﻿namespace mark.davison.berlin.api.orchestrator.Cron;

public class UpdateStoriesCron : BaseCQRSCron<UpdateStoriesRequest, UpdateStoriesResponse, UpdateStoriesCron>
{
    public UpdateStoriesCron(
        IScheduleConfig<UpdateStoriesCron> scheduleConfig,
        IDateService dateService,
        IDistributedPubSub distributedPubSubService,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<UpdateStoriesCron> logger,
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
