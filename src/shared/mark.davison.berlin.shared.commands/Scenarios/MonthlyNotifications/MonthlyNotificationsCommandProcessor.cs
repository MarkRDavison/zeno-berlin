namespace mark.davison.berlin.shared.commands.Scenarios.MonthlyNotifications;

public class MonthlyNotificationsCommandProcessor : ICommandProcessor<MonthlyNotificationsCommandRequest, MonthlyNotificationsCommandResponse>
{
    private readonly IReadonlyRepository _repository;
    private readonly IDateService _dateService;
    private readonly INotificationCreationService _notificationCreationService;
    private readonly INotificationHub _notificationHub;

    public MonthlyNotificationsCommandProcessor(
        IReadonlyRepository repository,
        IDateService dateService,
        INotificationCreationService notificationCreationService,
        INotificationHub notificationHub)
    {
        _repository = repository;
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

        await using (_repository.BeginTransaction())
        {
            var stories = await _repository.QueryEntities<StoryUpdate>()
                .Include(_ => _.Story)
                .Where(_ =>
                    !_.Story!.Complete &&
                    _.Story!.UpdateTypeId == UpdateTypeConstants.MonthlyWithUpdateId &&
                    _.LastAuthored >= startRange)
                .OrderByDescending(_ => _.CurrentChapters)
                .GroupBy(_ => _.StoryId)
                .SelectMany(_ => _)
                .ToListAsync();

            foreach (var storyUpdates in stories.GroupBy(_ => _.StoryId))
            {
                var storyId = storyUpdates.Key;

                var story = storyUpdates.Select(_ => _.Story).FirstOrDefault(_ => _ != null);
                if (story == null) { continue; }

                var site = await _repository.GetEntityAsync<Site>(story.SiteId, cancellationToken); // TODO: Cache
                if (site == null) { continue; }

                var updates = storyUpdates.OrderByDescending(_ => _.CurrentChapters).ToList();

                var updateText = _notificationCreationService.CreateNotification(story, [], site);

                await _notificationHub.SendNotification(updateText);
                notificationSent = true;
                response.StoriesNotified++;
            }
        }

        if (!notificationSent)
        {
            response.Warnings.Add(ValidationMessages.NO_ITEMS);
        }

        return response;
    }
}
