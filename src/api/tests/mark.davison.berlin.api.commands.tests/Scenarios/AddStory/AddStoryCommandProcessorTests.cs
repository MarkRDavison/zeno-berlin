namespace mark.davison.berlin.api.commands.tests.Scenarios.AddStory;

public sealed class AddStoryCommandProcessorTests
{
    private readonly Guid _currentUserId = Guid.NewGuid();

    private Mock<IDateService> _dateService = default!;
    private Mock<ICurrentUserContext> _currentUserContext = default!;
    private Mock<IStoryInfoProcessor> _storyInfoProcessor = default!;
    private Mock<IFandomService> _fandomService = default!;
    private Mock<IAuthorService> _authorService = default!;

    private IDbContext<BerlinDbContext> _dbContext = default!;
    private AddStoryCommandProcessor _processor = default!;

    private Site _site = default!;

    [Before(Test)]
    public void BeforeTest()
    {
        _dateService = new();

        _currentUserContext = new();
        _currentUserContext
            .Setup(_ => _.UserId)
            .Returns(_currentUserId);

        _storyInfoProcessor = new();
        _fandomService = new();
        _authorService = new();

        _site = new()
        {
            Id = Guid.NewGuid(),
            UserId = _currentUserId,
            ShortName = "SOMESITE",
            Address = "https://somesite.org",
            Created = DateTime.UtcNow,
            LastModified = DateTime.UtcNow
        };

        _dbContext = DbContextHelpers.CreateInMemory<BerlinDbContext>(_ => new(_));
        _dbContext.AddSync(_site);

        var services = new ServiceCollection();
        services.AddKeyedTransient(
            _site.ShortName,
            (_, __) => _storyInfoProcessor.Object);

        _processor = new AddStoryCommandProcessor(
            _dateService.Object,
            _dbContext,
            _fandomService.Object,
            _authorService.Object,
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
    public async Task ProcessAsync_CreatesStoryAndStoryUpdate()
    {
        const string externalId = "1234";

        var request = new AddStoryCommandRequest
        {
            SiteId = _site.Id,
            StoryAddress = $"{_site.Address}/works/{externalId}"
        };

        var storyInfo = new StoryInfoModel
        {
            Name = $"story-{externalId}",
            CurrentChapters = 10,
            TotalChapters = 10,
            IsCompleted = false,
            ChapterInfo = new()
            {
                { 10, new ChapterInfoModel(10, $"{request.StoryAddress}/chapters/10", "10") }
            }
        };

        _storyInfoProcessor
            .Setup(_ => _.ExtractExternalStoryId(
                request.StoryAddress,
                _site.Address))
            .Returns(externalId);

        _storyInfoProcessor
            .Setup(_ => _.GenerateBaseStoryAddress(
                request.StoryAddress,
                _site.Address))
            .Returns(request.StoryAddress);

        _storyInfoProcessor
            .Setup(_ => _.ExtractStoryInfo(
                request.StoryAddress,
                _site.Address,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(storyInfo);

        _fandomService
            .Setup(_ => _.GetOrCreateFandomsByExternalNames(
                It.IsAny<List<string>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Fandom>());

        _authorService
            .Setup(_ => _.GetOrCreateAuthorsByName(
                It.IsAny<List<string>>(),
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Author>());

        var response = await _processor.ProcessAsync(
            request,
            _currentUserContext.Object,
            CancellationToken.None);

        await Assert.That(response.Success).IsTrue();

        var storyExists = await _dbContext
            .Set<Story>()
            .AsNoTracking()
            .Where(_ =>
                _.ExternalId == externalId &&
                _.Name == storyInfo.Name &&
                _.CurrentChapters == storyInfo.CurrentChapters &&
                _.TotalChapters == storyInfo.TotalChapters &&
                _.Complete == storyInfo.IsCompleted)
            .CountAsync();

        await Assert.That(storyExists).IsEqualTo(1);

        var storyUpdateExists = await _dbContext
            .Set<StoryUpdate>()
            .AsNoTracking()
            .Where(_ =>
                _.StoryId != Guid.Empty &&
                _.CurrentChapters == storyInfo.CurrentChapters &&
                _.TotalChapters == storyInfo.TotalChapters &&
                _.Complete == storyInfo.IsCompleted &&
                _.ChapterTitle == "10" &&
                _.ChapterAddress == $"{request.StoryAddress}/chapters/10")
            .CountAsync();

        await Assert.That(storyUpdateExists).IsEqualTo(1);
    }
}