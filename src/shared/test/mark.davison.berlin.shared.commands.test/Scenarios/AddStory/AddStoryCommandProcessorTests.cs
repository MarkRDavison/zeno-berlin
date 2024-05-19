namespace mark.davison.berlin.shared.commands.test.Scenarios.AddStory;

[TestClass]
public sealed class AddStoryCommandProcessorTests
{
    private readonly IDateService _dateService;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IStoryInfoProcessor _storyInfoProcessor;
    private readonly IFandomService _fandomService;
    private readonly IAuthorService _authorService;

    private IDbContext<BerlinDbContext> _dbContext = default!;
    private AddStoryCommandProcessor _processor = default!;


    private readonly Site _site;
    private readonly User _user;

    public AddStoryCommandProcessorTests()
    {
        _dateService = Substitute.For<IDateService>();
        _currentUserContext = Substitute.For<ICurrentUserContext>();
        _storyInfoProcessor = Substitute.For<IStoryInfoProcessor>();
        _fandomService = Substitute.For<IFandomService>();
        _authorService = Substitute.For<IAuthorService>();

        _site = new()
        {
            Id = Guid.NewGuid(),
            ShortName = "SOMESITE",
            Address = "https://somesite.org"
        };
        _user = new()
        {
            Id = Guid.NewGuid()
        };

        _currentUserContext
            .CurrentUser
            .Returns(_user);

    }
    [TestInitialize]
    public void Initialize()
    {
        _dbContext = DbContextHelpers.CreateInMemory<BerlinDbContext>(_ => new(_));
        _dbContext.Add(_site);

        var services = new ServiceCollection();
        services.AddKeyedTransient(_site.ShortName, (_, __) => _storyInfoProcessor);

        _processor = new AddStoryCommandProcessor(
            _dateService,
            _dbContext,
            _fandomService,
            _authorService,
            services.BuildServiceProvider());
    }

    [TestMethod]
    public async Task ProcessAsync_CreatesStoryAndStoryUpdate()
    {
        const string externalId = "1234";
        var request = new AddStoryCommandRequest
        {
            SiteId = _site.Id,
            StoryAddress = _site.Address + "/works/" + externalId
        };

        var storyInfo = new StoryInfoModel
        {
            Name = "story-" + externalId,
            CurrentChapters = 10,
            TotalChapters = 10,
            IsCompleted = false,
            ChapterInfo = new()
            {
                { 10, new ChapterInfoModel(10, request.StoryAddress + "/chapters/10", "10") }
            }
        };

        _storyInfoProcessor
            .ExtractExternalStoryId(request.StoryAddress, _site.Address)
                .Returns(externalId);

        _storyInfoProcessor
            .GenerateBaseStoryAddress(request.StoryAddress, _site.Address)
                .Returns(request.StoryAddress);

        _storyInfoProcessor
            .ExtractStoryInfo(
                request.StoryAddress,
                _site.Address,
                Arg.Any<CancellationToken>())
            .Returns(_ => storyInfo);

        _fandomService
            .GetOrCreateFandomsByExternalNames(
                Arg.Any<List<string>>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new List<Fandom>()));

        _authorService
            .GetOrCreateAuthorsByName(
                Arg.Any<List<string>>(),
                Arg.Any<Guid>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new List<Author>()));

        var response = await _processor.ProcessAsync(request, _currentUserContext, CancellationToken.None);

        Assert.IsTrue(response.Success);

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

        Assert.AreEqual(1, storyExists);

        var storyUpdateExists = await _dbContext
            .Set<StoryUpdate>()
            .AsNoTracking()
            .Where(_ =>
                    _.StoryId != Guid.Empty &&
                    _.CurrentChapters == storyInfo.CurrentChapters &&
                    _.TotalChapters == storyInfo.TotalChapters &&
                    _.Complete == storyInfo.IsCompleted &&
                    _.ChapterTitle == "10" &&
                    _.ChapterAddress == request.StoryAddress + "/chapters/10")
            .CountAsync();

        Assert.AreEqual(1, storyUpdateExists);
    }
}
