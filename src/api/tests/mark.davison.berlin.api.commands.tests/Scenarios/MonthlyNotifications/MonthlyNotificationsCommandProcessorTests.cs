namespace mark.davison.berlin.api.commands.tests.Scenarios.MonthlyNotifications;

public sealed class MonthlyNotificationsCommandProcessorTests
{
    private readonly Guid _currentUserId = Guid.NewGuid();

    private Mock<IDateService> _dateService = default!;
    private Mock<ICurrentUserContext> _currentUserContext = default!;
    private Mock<INotificationCreationService> _notificationCreationService = default!;
    private Mock<INotificationHub> _notificationHub = default!;
    private DateOnly _currentDate;

    private IDbContext<BerlinDbContext> _dbContext = default!;
    private MonthlyNotificationsCommandProcessor _processor = default!;

    [Before(Test)]
    public void BeforeTest()
    {
        _currentDate = DateOnly.FromDateTime(DateTime.Today);

        _dateService = new();
        _currentUserContext = new();
        _notificationCreationService = new();
        _notificationHub = new();

        _currentUserContext.Setup(_ => _.UserId).Returns(_currentUserId);
        _dateService.Setup(_ => _.Today).Returns(_currentDate);

        _dbContext = DbContextHelpers.CreateInMemory<BerlinDbContext>(_ => new(_));
        _processor = new(_dbContext, _dateService.Object, _notificationCreationService.Object, _notificationHub.Object);
    }

    [After(Test)]
    public void Teardown()
    {
        if (_dbContext is BerlinDbContext db)
        {
            db.Dispose();
        }
    }

    [Test]
    public async Task ProcessAsync_ForValidStoriesTakesMostRecentUpdate_IfInRangeCreatesNotification()
    {
        var story1Id = Guid.NewGuid();
        var story2Id = Guid.NewGuid();

        var validDate = _currentDate.AddMonths(-1);
        var invalidDate = _currentDate.AddMonths(-2);

        var stories = new List<Story>
        {
            new Story { Id = story1Id, UserId = _currentUserId, Name = "Story 1", UpdateTypeId = UpdateTypeConstants.MonthlyWithUpdateId, Complete = false, Created = DateTime.UtcNow, LastModified = DateTime.UtcNow },
            new Story { Id = story2Id, UserId = _currentUserId, Name = "Story 2", UpdateTypeId = UpdateTypeConstants.MonthlyWithUpdateId, Complete = false, Created = DateTime.UtcNow, LastModified = DateTime.UtcNow }
        };

        var updates = new List<StoryUpdate>
        {
            new StoryUpdate { Id = Guid.NewGuid(), UserId = _currentUserId, StoryId = story1Id, CurrentChapters = 5, TotalChapters = null, LastAuthored = invalidDate, Story = stories.First(_ => _.Id == story1Id), Created = DateTime.UtcNow, LastModified = DateTime.UtcNow },
            new StoryUpdate { Id = Guid.NewGuid(), UserId = _currentUserId, StoryId = story1Id, CurrentChapters = 6, TotalChapters = null, LastAuthored = validDate, Story = stories.First(_ => _.Id == story1Id), Created = DateTime.UtcNow, LastModified = DateTime.UtcNow },
            new StoryUpdate { Id = Guid.NewGuid(), UserId = _currentUserId, StoryId = story1Id, CurrentChapters = 7, TotalChapters = null, LastAuthored = validDate, Story = stories.First(_ => _.Id == story1Id), Created = DateTime.UtcNow, LastModified = DateTime.UtcNow },
            new StoryUpdate { Id = Guid.NewGuid(), UserId = _currentUserId, StoryId = story2Id, CurrentChapters = 10, TotalChapters = null, LastAuthored = invalidDate, Story = stories.First(_ => _.Id == story2Id), Created = DateTime.UtcNow, LastModified = DateTime.UtcNow },
            new StoryUpdate { Id = Guid.NewGuid(), UserId = _currentUserId, StoryId = story2Id, CurrentChapters = 11, TotalChapters = null, LastAuthored = validDate, Story = stories.First(_ => _.Id == story2Id), Created = DateTime.UtcNow, LastModified = DateTime.UtcNow }
        };

        _dbContext.AddSync(updates);

        var response = await _processor.ProcessAsync(new(), _currentUserContext.Object, CancellationToken.None);

        await Assert.That(response.Success).IsTrue();
    }
}