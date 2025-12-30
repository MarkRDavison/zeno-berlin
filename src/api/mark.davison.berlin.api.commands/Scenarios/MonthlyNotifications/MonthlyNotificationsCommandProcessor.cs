namespace mark.davison.berlin.api.commands.Scenarios.MonthlyNotifications;

public sealed class MonthlyNotificationsCommandProcessor : ICommandProcessor<MonthlyNotificationsCommandRequest, MonthlyNotificationsCommandResponse>
{
    private readonly IDbContext<BerlinDbContext> _dbContext;
    private readonly IDateService _dateService;
    private readonly INotificationCreationService _notificationCreationService;
    private readonly INotificationHub _notificationHub;

    public MonthlyNotificationsCommandProcessor(
        IDbContext<BerlinDbContext> dbContext,
        IDateService dateService,
        INotificationCreationService notificationCreationService,
        INotificationHub notificationHub)
    {
        _dbContext = dbContext;
        _dateService = dateService;
        _notificationCreationService = notificationCreationService;
        _notificationHub = notificationHub;
    }

    public async Task<MonthlyNotificationsCommandResponse> ProcessAsync(MonthlyNotificationsCommandRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var response = new MonthlyNotificationsCommandResponse();

        var today = _dateService.Today;

        if (5 < today.Day && today.Day < 25)
        {
            // skips the 15th....
            return new();
        }

        var startRange = today.AddMonths(-1);

        bool notificationSent = false;

        var stories = await _dbContext
            .Set<StoryUpdate>()
            .AsNoTracking()
            .Include(_ => _.Story)
            .Where(_ =>
                !_.Story!.Complete &&
                _.Story!.UpdateTypeId == UpdateTypeConstants.MonthlyWithUpdateId &&
                _.LastAuthored >= startRange)
            .OrderByDescending(_ => _.CurrentChapters)
            .ToListAsync();

        foreach (var storyUpdates in stories.GroupBy(_ => _.StoryId))
        {
            var storyId = storyUpdates.Key;

            var story = storyUpdates.Select(_ => _.Story).FirstOrDefault(_ => _ != null);
            if (story == null) { continue; }

            var site = await _dbContext.Set<Site>().FindAsync(story.SiteId, cancellationToken); // TODO: Cache
            if (site == null) { continue; }

            var updates = storyUpdates.OrderByDescending(_ => _.CurrentChapters).ToList();

            var updateText = _notificationCreationService.CreateNotification(story, [], site);

            await _notificationHub.SendNotification(new NotificationMessage { Message = updateText });
            notificationSent = true;
            response.StoriesNotified++;
        }

        if (!notificationSent)
        {
            response.Warnings.Add(ValidationMessages.NO_ITEMS);
        }

        return response;
    }
}
