namespace mark.davison.berlin.shared.commands.test.Scenarios.MonthlyNotifications;

[TestClass]
public sealed class MonthlyNotificationsCommandProcessorTests
{
    private readonly IDateService _dateService;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly INotificationCreationService _notificationCreationService;
    private readonly INotificationHub _notificationHub;
    private readonly DateOnly _currentDate;

    private IDbContext<BerlinDbContext> _dbContext = default!;
    private MonthlyNotificationsCommandProcessor _processor = default!;

    public MonthlyNotificationsCommandProcessorTests()
    {
        _currentDate = DateOnly.FromDateTime(DateTime.Today);
        _dateService = Substitute.For<IDateService>();
        _currentUserContext = Substitute.For<ICurrentUserContext>();
        _notificationCreationService = Substitute.For<INotificationCreationService>();
        _notificationHub = Substitute.For<INotificationHub>();

        _dateService.Today.Returns(_currentDate);
    }

    [TestInitialize]
    public void Initialize()
    {
        _dbContext = DbContextHelpers.CreateInMemory<BerlinDbContext>(_ => new(_));
        _processor = new(_dbContext, _dateService, _notificationCreationService, _notificationHub);
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
                Id = Guid.NewGuid(),
                StoryId = story1Id,
                CurrentChapters = 5,
                TotalChapters = null,
                LastAuthored = invalidDate
            },
            new StoryUpdate
            {
                Id = Guid.NewGuid(),
                StoryId = story1Id,
                CurrentChapters = 6,
                TotalChapters = null,
                LastAuthored = validDate
            },
            new StoryUpdate
            {
                Id = Guid.NewGuid(),
                StoryId = story1Id,
                CurrentChapters = 7,
                TotalChapters = null,
                LastAuthored = validDate
            },
            new StoryUpdate
            {
                Id = Guid.NewGuid(),
                StoryId = story2Id,
                CurrentChapters = 10,
                TotalChapters = null,
                LastAuthored = invalidDate
            },
            new StoryUpdate
            {
                Id = Guid.NewGuid(),
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

        _dbContext.AddSync(updates);

        var response = await _processor.ProcessAsync(new(), _currentUserContext, CancellationToken.None);

        Assert.IsTrue(response.Success);
    }
}
