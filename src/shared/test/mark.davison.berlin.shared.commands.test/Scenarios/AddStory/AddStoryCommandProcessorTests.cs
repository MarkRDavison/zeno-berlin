namespace mark.davison.berlin.shared.commands.test.Scenarios.AddStory;

[TestClass]
public sealed class AddStoryCommandProcessorTests
{
    private readonly IDateService _dateService;
    private readonly IValidationContext _validationContext;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IStoryInfoProcessor _storyInfoProcessor;
    private readonly IFandomService _fandomService;
    private readonly IAuthorService _authorService;
    private readonly IRepository _repository;
    private readonly AddStoryCommandProcessor _processor;

    private readonly Site _site;
    private readonly User _user;

    public AddStoryCommandProcessorTests()
    {
        _dateService = Substitute.For<IDateService>();
        _validationContext = Substitute.For<IValidationContext>();
        _currentUserContext = Substitute.For<ICurrentUserContext>();
        _storyInfoProcessor = Substitute.For<IStoryInfoProcessor>();
        _fandomService = Substitute.For<IFandomService>();
        _authorService = Substitute.For<IAuthorService>();
        _repository = Substitute.For<IRepository>();

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

        _validationContext
            .GetAll<Site>(Arg.Any<CancellationToken>())
            .Returns(_ => Task.FromResult(new List<Site> { _site }));
        _validationContext
            .GetById<Site>(_site.Id, Arg.Any<CancellationToken>())
            .Returns(_ => Task.FromResult<Site?>(_site));

        _repository
            .BeginTransaction()
            .Returns(new StubAsyncDisposable());

        var services = new ServiceCollection();
        services.AddScoped(_ => _repository);
        services.AddKeyedTransient(_site.ShortName, (_, __) => _storyInfoProcessor);

        _processor = new AddStoryCommandProcessor(
            _dateService,
            _validationContext,
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
            .ExtractExternalStoryId(request.StoryAddress)
                .Returns(externalId);

        _storyInfoProcessor
            .GenerateBaseStoryAddress(request.StoryAddress)
                .Returns(request.StoryAddress);

        _storyInfoProcessor
            .ExtractStoryInfo(
                request.StoryAddress,
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

        _repository
            .UpsertEntityAsync<Story>(
                Arg.Any<Story>())
            .Returns(_ => Task.FromResult<Story?>(_.Arg<Story>()));

        _repository
            .UpsertEntityAsync<StoryUpdate>(
                Arg.Any<StoryUpdate>())
            .Returns(_ => Task.FromResult<StoryUpdate?>(_.Arg<StoryUpdate>()));

        var response = await _processor.ProcessAsync(request, _currentUserContext, CancellationToken.None);

        Assert.IsTrue(response.Success);

        await _repository
            .Received(1)
            .UpsertEntityAsync<Story>(
                Arg.Is<Story>(_ =>
                    _.ExternalId == externalId &&
                    _.Name == storyInfo.Name &&
                    _.CurrentChapters == storyInfo.CurrentChapters &&
                    _.TotalChapters == storyInfo.TotalChapters &&
                    _.Complete == storyInfo.IsCompleted));

        await _repository
            .Received(1)
            .UpsertEntityAsync<StoryUpdate>(
                Arg.Is<StoryUpdate>(_ =>
                    _.StoryId != Guid.Empty &&
                    _.CurrentChapters == storyInfo.CurrentChapters &&
                    _.TotalChapters == storyInfo.TotalChapters &&
                    _.Complete == storyInfo.IsCompleted &&
                    _.ChapterTitle == "10" &&
                    _.ChapterAddress == request.StoryAddress + "/chapters/10"));
    }
}
