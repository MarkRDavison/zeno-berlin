namespace mark.davison.berlin.api.commands.tests.Scenarios.UpdateStories;

public sealed class UpdateStoriesCommandProcessorTests
{
    private readonly Guid _currentUserId = Guid.NewGuid();

    private Mock<ILogger<UpdateStoriesCommandProcessor>> _logger = default!;
    private Mock<IDateService> _dateService = default!;
    private Mock<INotificationHub> _notificationHub = default!;
    private Mock<IFandomService> _fandomService = default!;
    private Mock<IAuthorService> _authorService = default!;
    private Mock<ICurrentUserContext> _currentUserContext = default!;
    private Mock<INotificationCreationService> _notificationCreationService = default!;
    private IOptions<Ao3Config> _ao3Config = default!;

    private IDbContext<BerlinDbContext> _dbContext = default!;
    private UpdateStoriesCommandProcessor _processor = default!;

    private Mock<IStoryInfoProcessor> _site1StoryInfoProcessor = default!;
    private Mock<IStoryInfoProcessor> _site2StoryInfoProcessor = default!;

    private Site _site1 = default!;
    private Site _site2 = default!;
    private Story _story1Site1 = default!;
    private Story _story1Site2 = default!;
    private Story _story2Site1 = default!;
    private Story _story2Site2 = default!;

    [Before(Test)]
    public void BeforeTest()
    {
        _logger = new();
        _dateService = new();
        _notificationHub = new();
        _fandomService = new();
        _authorService = new();
        _notificationCreationService = new();
        _currentUserContext = new();
        _ao3Config = Options.Create(new Ao3Config());

        _currentUserContext.Setup(_ => _.UserId).Returns(_currentUserId);

        _site1StoryInfoProcessor = new();
        _site2StoryInfoProcessor = new();

        _dbContext = DbContextHelpers.CreateInMemory<BerlinDbContext>(_ => new(_));

        _dateService.Setup(_ => _.Now).Returns(DateTime.Now);

        _site1 = new()
        {
            Id = Guid.NewGuid(),
            UserId = _currentUserId,
            ShortName = "SOMESITE1",
            Address = "https://somesite1.org",
            Created = DateTime.UtcNow,
            LastModified = DateTime.UtcNow
        };
        _site2 = new()
        {
            Id = Guid.NewGuid(),
            UserId = _currentUserId,
            ShortName = "SOMESITE2",
            Address = "https://somesite2.org",
            Created = DateTime.UtcNow,
            LastModified = DateTime.UtcNow
        };
        _dbContext.AddSync(_site1);
        _dbContext.AddSync(_site2);

        _story1Site1 = new()
        {
            Id = Guid.NewGuid(),
            UserId = _currentUserId,
            SiteId = _site1.Id,
            LastModified = _dateService.Object.Now.AddDays(-2),
            Created = DateTime.UtcNow
        };
        _story2Site1 = new()
        {
            Id = Guid.NewGuid(),
            UserId = _currentUserId,
            SiteId = _site1.Id,
            LastModified = _dateService.Object.Now.AddDays(-1.5),
            Created = DateTime.UtcNow
        };
        _story1Site2 = new()
        {
            Id = Guid.NewGuid(),
            UserId = _currentUserId,
            SiteId = _site2.Id,
            LastModified = _dateService.Object.Now,
            Created = DateTime.UtcNow
        };
        _story2Site2 = new()
        {
            Id = Guid.NewGuid(),
            UserId = _currentUserId,
            SiteId = _site2.Id,
            LastModified = _dateService.Object.Now,
            Created = DateTime.UtcNow
        };

        var services = new ServiceCollection();
        services.AddKeyedTransient(_site1.ShortName, (_, __) => _site1StoryInfoProcessor.Object);
        services.AddKeyedTransient(_site2.ShortName, (_, __) => _site2StoryInfoProcessor.Object);

        _processor = new(
            _logger.Object,
            _dbContext,
            _dateService.Object,
            _notificationHub.Object,
            _fandomService.Object,
            _authorService.Object,
            _notificationCreationService.Object,
            _ao3Config,
            services.BuildServiceProvider());
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
    public async Task GetStoriesToUpdate_WhereNoStoryIdsProvided_QueriesStoriesBasedOnLastModified()
    {
        var stories = new List<Story> { _story1Site1, _story1Site2, _story2Site1, _story2Site2 };

        stories[0].LastModified = _dateService.Object.Now.AddDays(-3);
        stories[1].LastModified = _dateService.Object.Now.AddDays(-2);
        stories[2].LastModified = _dateService.Object.Now.AddDays(-1);
        stories[3].LastModified = _dateService.Object.Now;

        _dbContext.AddSync(stories);

        var request = new UpdateStoriesRequest();

        var queried = await _processor.GetStoriesToUpdate(request, CancellationToken.None);

        await Assert.That(queried.Count).IsEqualTo(2);
    }

    [Test]
    public async Task ProcessAsync_WhereStoryIdsProvided_QueriesStoriesBasedOnProvidedIds()
    {
        var stories = new List<Story> { _story1Site1, _story1Site2, _story2Site1, _story2Site2 };
        var request = new UpdateStoriesRequest { StoryIds = stories.Select(_ => _.Id).ToList() };

        var response = await _processor.ProcessAsync(request, _currentUserContext.Object, CancellationToken.None);

        await Assert.That(response.Success).IsTrue();
        await Assert.That(response.Warnings.Any(_ => _ == ValidationMessages.NO_ITEMS)).IsTrue();
    }

    [Test]
    public async Task ProcessAsync_InvokesStoryInfoProcessor_PerSiteGrouping()
    {
        var stories = new List<Story> { _story1Site1, _story1Site2, _story2Site1, _story2Site2 };
        var request = new UpdateStoriesRequest { StoryIds = stories.Select(_ => _.Id).ToList() };

        _dbContext.AddSync(stories);

        _site1StoryInfoProcessor.Setup(_ => _.ExtractStoryInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new Response<StoryInfoModel> { Value = new StoryInfoModel() });
        _site2StoryInfoProcessor.Setup(_ => _.ExtractStoryInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new Response<StoryInfoModel> { Value = new StoryInfoModel() });

        var response = await _processor.ProcessAsync(request, _currentUserContext.Object, CancellationToken.None);

        await Assert.That(response.Success).IsTrue();
    }

    [Test]
    public async Task ProcessAsync_UpdatesStoryInfoOnStories()
    {
        var request = new UpdateStoriesRequest { StoryIds = new List<Guid> { _story1Site1.Id } };
        _dbContext.AddSync(_story1Site1);

        var info = new StoryInfoModel { };

        _site1StoryInfoProcessor.Setup(_ => _.ExtractStoryInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new Response<StoryInfoModel> { Value = info });

        var response = await _processor.ProcessAsync(request, _currentUserContext.Object, CancellationToken.None);

        await Assert.That(response.Success).IsTrue();

        var stories = await _dbContext.Set<Story>().AsNoTracking().ToListAsync();
        await Assert.That(stories.Count).IsEqualTo(1);

        var story = stories.Single();
        await Assert.That(story.Id).IsEqualTo(_story1Site1.Id);
        await Assert.That(story.TotalChapters).IsEqualTo(info.TotalChapters);
        await Assert.That(story.CurrentChapters).IsEqualTo(info.CurrentChapters);
        await Assert.That(story.Complete).IsEqualTo(info.IsCompleted);
        await Assert.That(story.Name).IsEqualTo(info.Name);
    }

    [Test]
    public async Task ProcessAsync_SendsNotification_ForUpdatedStory()
    {
        var request = new UpdateStoriesRequest { StoryIds = new List<Guid> { _story1Site1.Id } };
        _dbContext.AddSync(_story1Site1);

        var info = new StoryInfoModel { Name = "A new name" };
        _site1StoryInfoProcessor.Setup(_ => _.ExtractStoryInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new Response<StoryInfoModel> { Value = info });

        _notificationHub.Setup(_ => _.SendNotification(It.IsAny<NotificationMessage>())).ReturnsAsync(new Response());

        var response = await _processor.ProcessAsync(request, _currentUserContext.Object, CancellationToken.None);

        await Assert.That(response.Success).IsTrue();
    }

    [Test]
    public async Task ProcessAsync_WhereNewUpdateSkipsChapters_AddsMissingChapterUpdates()
    {
        var request = new UpdateStoriesRequest { StoryIds = new List<Guid> { _story1Site1.Id } };
        _dbContext.AddSync(_story1Site1);

        var info = new StoryInfoModel { Name = "A new name", CurrentChapters = 10 };
        var missingChapters = 5;

        var existingUpdate = new StoryUpdate
        {
            Id = Guid.NewGuid(),
            UserId = _currentUserId,
            StoryId = _story1Site1.Id,
            CurrentChapters = info.CurrentChapters - missingChapters,
            Created = DateTime.UtcNow,
            LastModified = DateTime.UtcNow
        };
        _dbContext.AddSync(existingUpdate);

        _notificationHub.Setup(_ => _.SendNotification(It.IsAny<NotificationMessage>())).ReturnsAsync(new Response());
        _site1StoryInfoProcessor.Setup(_ => _.ExtractStoryInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new Response<StoryInfoModel> { Value = info });

        var response = await _processor.ProcessAsync(request, _currentUserContext.Object, CancellationToken.None);
        await Assert.That(response.Success).IsTrue();

        var storyUpdates = await _dbContext.Set<StoryUpdate>().AsNoTracking().Where(_ => existingUpdate.Id != _.Id).ToListAsync();
        await Assert.That(storyUpdates.Count).IsEqualTo(missingChapters);
    }

    [Test]
    public async Task ProcessAsync_WhereNewUpdates_AppliesChapterInfoCorrectly()
    {
        var request = new UpdateStoriesRequest { StoryIds = new List<Guid> { _story1Site1.Id } };
        _dbContext.AddSync(_story1Site1);

        var info = new StoryInfoModel
        {
            Name = "A new name",
            CurrentChapters = 10,
            ChapterInfo = new() { { 10, new ChapterInfoModel(10, "/chapters/10", "10") }, { 7, new ChapterInfoModel(7, "/chapters/7", "7") } }
        };
        var missingChapters = 5;

        var existingUpdate = new StoryUpdate
        {
            Id = Guid.NewGuid(),
            StoryId = _story1Site1.Id,
            UserId = _currentUserId,
            CurrentChapters = info.CurrentChapters - missingChapters,
            Created = DateTime.UtcNow,
            LastModified = DateTime.UtcNow
        };
        _dbContext.AddSync(existingUpdate);

        _notificationHub.Setup(_ => _.SendNotification(It.IsAny<NotificationMessage>())).ReturnsAsync(new Response());
        _site1StoryInfoProcessor.Setup(_ => _.ExtractStoryInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new Response<StoryInfoModel> { Value = info });

        var response = await _processor.ProcessAsync(request, _currentUserContext.Object, CancellationToken.None);
        await Assert.That(response.Success).IsTrue();

        var storyUpdates = await _dbContext.Set<StoryUpdate>().AsNoTracking().Where(_ => existingUpdate.Id != _.Id).ToListAsync();
        await Assert.That(storyUpdates.Count).IsEqualTo(missingChapters);

        foreach (var update in storyUpdates)
        {
            if (info.ChapterInfo.TryGetValue(update.CurrentChapters, out var chapterInfo))
            {
                await Assert.That(update.ChapterAddress).IsEqualTo(chapterInfo.Address);
                await Assert.That(update.ChapterTitle).IsEqualTo(chapterInfo.Title);
            }
            else
            {
                await Assert.That(update.ChapterAddress).IsNull();
                await Assert.That(update.ChapterTitle).IsNull();
            }
        }
    }

    [Test]
    public async Task GetStoriesToUpdate_WhereStoryRequiresAuthentication_DoesNotUpdateStory()
    {
        _story1Site1.RequiresAuthentication = true;
        _story1Site1.LastModified.AddDays(-3);

        _dbContext.AddSync([_story1Site1]);

        var request = new UpdateStoriesRequest();

        var queried = await _processor.GetStoriesToUpdate(request, CancellationToken.None);

        await Assert.That(queried.Count).IsEqualTo(0);
    }

    [Test]
    public async Task ProcessAsync_WhereStoryWillRequireAuthentication_MutatesStory()
    {
        _story1Site1.LastModified.AddDays(-3);

        _dbContext.AddSync([_story1Site1]);

        _site1StoryInfoProcessor
            .Setup(_ => _.ExtractStoryInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Response<StoryInfoModel>
            {
                Errors = [ValidationMessages.AUTHENTICATION_REQUIRED]
            });

        var request = new UpdateStoriesRequest();

        await _processor.ProcessAsync(request, _currentUserContext.Object, CancellationToken.None);

        var persistedStory = await _dbContext
            .Set<Story>()
            .FirstAsync(_ => _.Id == _story1Site1.Id);

        await Assert.That(persistedStory.RequiresAuthentication).IsTrue();
    }
}