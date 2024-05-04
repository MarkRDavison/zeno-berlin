namespace mark.davison.berlin.shared.commands.test.Scenarios.MonthlyNotifications;

[TestClass]
public class MonthlyNotificationsCommandProcessorTests
{
    private readonly IReadonlyRepository _repository;
    private readonly IDateService _dateService;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly INotificationCreationService _notificationCreationService;
    private readonly INotificationHub _notificationHub;
    private readonly DateOnly _currentDate;

    private readonly MonthlyNotificationsCommandProcessor _processor;

    public MonthlyNotificationsCommandProcessorTests()
    {
        _currentDate = DateOnly.FromDateTime(DateTime.Today);
        _repository = Substitute.For<IReadonlyRepository>();
        _dateService = Substitute.For<IDateService>();
        _currentUserContext = Substitute.For<ICurrentUserContext>();
        _notificationCreationService = Substitute.For<INotificationCreationService>();
        _notificationHub = Substitute.For<INotificationHub>();

        _dateService.Today.Returns(_currentDate);

        _processor = new(_repository, _dateService, _notificationCreationService, _notificationHub);
    }

    [TestMethod]
    public async Task ProcessAsync_ForValidStoriesTakesMostRecentUpdate_IfInRangeCreatesNotification()
    {
        var story1Id = Guid.NewGuid();
        var story2Id = Guid.NewGuid();

        var validDate = _currentDate.AddMonths(-1);
        var invalidDate = _currentDate.AddMonths(-2);

        var stories = new List<Story>
        {
            new Story
            {
                Id = story1Id,
                Name = "Story 1",
                UpdateTypeId = UpdateTypeConstants.MonthlyWithUpdateId,
                Complete = false
            },
            new Story
            {
                Id = story2Id,
                Name = "Story 2",
                UpdateTypeId = UpdateTypeConstants.MonthlyWithUpdateId,
                Complete = false
            }
        };
        var updates = new List<StoryUpdate>
        {
            new StoryUpdate
            {
                StoryId = story1Id,
                CurrentChapters = 5,
                TotalChapters = null,
                LastAuthored = invalidDate
            },
            new StoryUpdate
            {
                StoryId = story1Id,
                CurrentChapters = 6,
                TotalChapters = null,
                LastAuthored = validDate
            },
            new StoryUpdate
            {
                StoryId = story1Id,
                CurrentChapters = 7,
                TotalChapters = null,
                LastAuthored = validDate
            },
            new StoryUpdate
            {
                StoryId = story2Id,
                CurrentChapters = 10,
                TotalChapters = null,
                LastAuthored = invalidDate
            },
            new StoryUpdate
            {
                StoryId = story2Id,
                CurrentChapters = 11,
                TotalChapters = null,
                LastAuthored = validDate
            }
        };

        foreach (var update in updates)
        {
            var story = stories.FirstOrDefault(_ => _.Id == update.StoryId);

            update.Story = story;
        }

        _repository
            .QueryEntities<StoryUpdate>()
            .Returns(updates.AsAsyncQueryable());

        var response = await _processor.ProcessAsync(new(), _currentUserContext, CancellationToken.None);

        Assert.IsTrue(response.Success);
    }
}
