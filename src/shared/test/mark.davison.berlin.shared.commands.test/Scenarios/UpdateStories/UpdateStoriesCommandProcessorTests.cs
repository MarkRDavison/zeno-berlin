namespace mark.davison.berlin.shared.commands.test.Scenarios.UpdateStories;

[TestClass]
public sealed class UpdateStoriesCommandProcessorTests
{
    private readonly ILogger<UpdateStoriesCommandProcessor> _logger;
    private readonly IDateService _dateService;
    private readonly INotificationHub _notificationHub;
    private readonly IFandomService _fandomService;
    private readonly IAuthorService _authorService;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly INotificationCreationService _notificationCreationService;
    private readonly IOptions<Ao3Config> _ao3Config;
    private IDbContext<BerlinDbContext> _dbContext = default!;
    private UpdateStoriesCommandProcessor _processor = default!;

    private readonly IStoryInfoProcessor _site1StoryInfoProcessor;
    private readonly IStoryInfoProcessor _site2StoryInfoProcessor;

    private readonly User _user;
    private readonly Site _site1;
    private readonly Site _site2;
    private readonly Story _story1Site1;
    private readonly Story _story1Site2;
    private readonly Story _story2Site1;
    private readonly Story _story2Site2;

    public UpdateStoriesCommandProcessorTests()
    {
        _logger = Substitute.For<ILogger<UpdateStoriesCommandProcessor>>();
        _dateService = Substitute.For<IDateService>();
        _site1StoryInfoProcessor = Substitute.For<IStoryInfoProcessor>();
        _site2StoryInfoProcessor = Substitute.For<IStoryInfoProcessor>();
        _notificationHub = Substitute.For<INotificationHub>();
        _fandomService = Substitute.For<IFandomService>();
        _authorService = Substitute.For<IAuthorService>();
        _notificationCreationService = Substitute.For<INotificationCreationService>();
        _ao3Config = Options.Create<Ao3Config>(new Ao3Config());
        _currentUserContext = Substitute.For<ICurrentUserContext>();

        _dbContext = DbContextHelpers.CreateInMemory<BerlinDbContext>(_ => new(_));

        _dateService.Now.Returns(DateTime.Now);

        _user = new User
        {
            Id = Guid.NewGuid()
        };
        _site1 = new()
        {
            Id = Guid.NewGuid(),
            ShortName = "SOMESITE1",
            Address = "https://somesite1.org"
        };
        _site2 = new()
        {
            Id = Guid.NewGuid(),
            ShortName = "SOMESITE2",
            Address = "https://somesite2.org"
        };
        _story1Site1 = new Story
        {
            Id = Guid.NewGuid(),
            SiteId = _site1.Id,
            LastModified = _dateService.Now.AddDays(-2)
        };
        _story2Site1 = new Story
        {
            Id = Guid.NewGuid(),
            SiteId = _site1.Id,
            LastModified = _dateService.Now.AddDays(-1.5)
        };
        _story1Site2 = new Story
        {
            Id = Guid.NewGuid(),
            SiteId = _site2.Id,
            LastModified = _dateService.Now
        };
        _story2Site2 = new Story
        {
            Id = Guid.NewGuid(),
            SiteId = _site2.Id,
            LastModified = _dateService.Now
        };

        _currentUserContext
            .CurrentUser
            .Returns(_ => _user);

        _notificationHub
            .SendNotification(Arg.Any<string>())
            .Returns(new Response());
    }

    [TestInitialize]
    public void Initialise()
    {
        _dbContext = DbContextHelpers.CreateInMemory<BerlinDbContext>(_ => new(_));
        _dbContext.AddSync([_site1, _site2]);

        var services = new ServiceCollection();
        services.AddKeyedTransient(_site1.ShortName, (_, __) => _site1StoryInfoProcessor);
        services.AddKeyedTransient(_site2.ShortName, (_, __) => _site2StoryInfoProcessor);

        _processor = new(
            _logger,
            _dbContext,
            _dateService,
            _notificationHub,
            _fandomService,
            _authorService,
            _notificationCreationService,
            _ao3Config,
            services.BuildServiceProvider());
    }

    [TestMethod]
    public async Task GetStoriesToUpdate_WhereNoStoryIdsProvided_QueriesStoriesBasedOnLastModified()
    {
        var stories = new List<Story>
        {
            _story1Site1,
            _story1Site2,
            _story2Site1,
            _story2Site2
        };

        stories[0].LastModified = _dateService.Now.AddDays(-3);
        stories[1].LastModified = _dateService.Now.AddDays(-2);
        stories[2].LastModified = _dateService.Now.AddDays(-1);
        stories[3].LastModified = _dateService.Now.AddDays(-0);

        _dbContext.AddSync(stories);

        var request = new UpdateStoriesRequest();

        var queried = await _processor.GetStoriesToUpdate(request, CancellationToken.None);

        Assert.AreEqual(2, queried.Count);
    }

    [TestMethod]
    public async Task ProcessAsync_WhereStoryIdsProvided_QueriesStoriesBasedOnProvidedIds()
    {
        var stories = new List<Story>
        {
            _story1Site1,
            _story1Site2,
            _story2Site1,
            _story2Site2
        };

        var request = new UpdateStoriesRequest
        {
            StoryIds = [.. stories.Select(_ => _.Id)]
        };

        var response = await _processor.ProcessAsync(request, _currentUserContext, CancellationToken.None);

        Assert.IsTrue(response.Success);
        Assert.IsTrue(response.Warnings.Any(_ => _ == ValidationMessages.NO_ITEMS));
    }

    [TestMethod]
    public async Task ProcessAsync_InvokesStoryInfoProcessor_PerSiteGrouping()
    {
        var stories = new List<Story>
        {
            _story1Site1,
            _story1Site2,
            _story2Site1,
            _story2Site2
        };

        var request = new UpdateStoriesRequest
        {
            StoryIds = [.. stories.Select(_ => _.Id)]
        };

        _dbContext.AddSync(stories);

        _site1StoryInfoProcessor
            .ExtractStoryInfo(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns(new StoryInfoModel());

        _site2StoryInfoProcessor
            .ExtractStoryInfo(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns(new StoryInfoModel());

        var response = await _processor.ProcessAsync(request, _currentUserContext, CancellationToken.None);

        Assert.IsTrue(response.Success);

        await _site1StoryInfoProcessor
            .Received(2)
            .ExtractStoryInfo(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>());

        await _site2StoryInfoProcessor
            .Received(2)
            .ExtractStoryInfo(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>());
    }

    [TestMethod]
    public async Task ProcessAsync_UpdatesStoryInfoOnStories()
    {
        var request = new UpdateStoriesRequest
        {
            StoryIds = [_story1Site1.Id]
        };

        _dbContext.AddSync(_story1Site1);

        var info = new StoryInfoModel { };

        _site1StoryInfoProcessor
            .ExtractStoryInfo(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns(info);

        var response = await _processor.ProcessAsync(request, _currentUserContext, CancellationToken.None);

        Assert.IsTrue(response.Success);

        var stories = await _dbContext
            .Set<Story>()
            .AsNoTracking()
            .ToListAsync();

        Assert.AreEqual(1, stories.Count);

        var story = stories.Single();

        Assert.AreEqual(_story1Site1.Id, story.Id);
        Assert.AreEqual(info.TotalChapters, story.TotalChapters);
        Assert.AreEqual(info.CurrentChapters, story.CurrentChapters);
        Assert.AreEqual(info.IsCompleted, story.Complete);
        Assert.AreEqual(info.Name, story.Name);
    }

    [TestMethod]
    public async Task ProcessAsync_SendsNotification_ForUpdatedStory()
    {
        var request = new UpdateStoriesRequest
        {
            StoryIds = [_story1Site1.Id]
        };

        _dbContext.AddSync(_story1Site1);

        var info = new StoryInfoModel
        {
            Name = "A new name"
        };

        _site1StoryInfoProcessor
            .ExtractStoryInfo(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns(info);

        _notificationHub
            .SendNotification(Arg.Any<string>())
            .Returns(new Response());

        var response = await _processor.ProcessAsync(request, _currentUserContext, CancellationToken.None);

        Assert.IsTrue(response.Success);

        await _notificationHub
            .Received(1)
            .SendNotification(Arg.Any<string>());
    }

    [TestMethod]
    public async Task ProcessAsync_WhereNewUpdateSkipsChapters_AddsMissingChapterUpdates()
    {
        var request = new UpdateStoriesRequest
        {
            StoryIds = [_story1Site1.Id]
        };

        _dbContext.AddSync(_story1Site1);

        var info = new StoryInfoModel
        {
            Name = "A new name",
            CurrentChapters = 10
        };

        var missingChapters = 5;

        var existingUpdate = new StoryUpdate
        {
            Id = Guid.NewGuid(),
            StoryId = _story1Site1.Id,
            CurrentChapters = info.CurrentChapters - missingChapters
        };

        _dbContext.AddSync(existingUpdate);

        _site1StoryInfoProcessor
            .ExtractStoryInfo(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns(info);

        var response = await _processor.ProcessAsync(request, _currentUserContext, CancellationToken.None);

        Assert.IsTrue(response.Success);

        var storyUpdates = await _dbContext
            .Set<StoryUpdate>()
            .AsNoTracking()
            .Where(_ => existingUpdate.Id != _.Id)
            .ToListAsync();

        Assert.AreEqual(missingChapters, storyUpdates.Count);
    }

    [TestMethod]
    public async Task ProcessAsync_WhereNewUpdates_AppliesChapterInfoCorrectly()
    {
        var request = new UpdateStoriesRequest
        {
            StoryIds = [_story1Site1.Id]
        };

        _dbContext.AddSync(_story1Site1);

        var info = new StoryInfoModel
        {
            Name = "A new name",
            CurrentChapters = 10,
            ChapterInfo = new()
           {
               { 10, new ChapterInfoModel(10, "/chapters/10", "10") },
               { 7, new ChapterInfoModel(7, "/chapters/7", "7") }
           }
        };

        var missingChapters = 5;

        var existingUpdate =
            new StoryUpdate
            {
                StoryId = _story1Site1.Id,
                CurrentChapters = info.CurrentChapters - missingChapters
            };

        _dbContext.AddSync(existingUpdate);

        _site1StoryInfoProcessor
            .ExtractStoryInfo(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns(info);

        var response = await _processor.ProcessAsync(request, _currentUserContext, CancellationToken.None);

        Assert.IsTrue(response.Success);

        var storyUpdates = await _dbContext
            .Set<StoryUpdate>()
            .AsNoTracking()
            .Where(_ => existingUpdate.Id != _.Id)
            .ToListAsync();

        Assert.AreEqual(missingChapters, storyUpdates.Count);

        foreach (var update in storyUpdates)
        {
            if (info.ChapterInfo.TryGetValue(update.CurrentChapters, out var chapterInfo))
            {
                Assert.AreEqual(chapterInfo.Address, update.ChapterAddress);
                Assert.AreEqual(chapterInfo.Title, update.ChapterTitle);
            }
            else
            {
                Assert.IsNull(update.ChapterAddress);
                Assert.IsNull(update.ChapterTitle);
            }
        }
    }
}
