namespace mark.davison.berlin.shared.commands.test.Scenarios.AddPotentialStory;

[TestClass]
public sealed class AddPotentialStoryCommandProcessorTests
{
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IStoryInfoProcessor _storyInfoProcessor;
    private readonly User _currentUser;

    private IDbContext<BerlinDbContext> _dbContext = default!;
    private AddPotentialStoryCommandProcessor _processor = default!;

    private readonly Site _site;
    public AddPotentialStoryCommandProcessorTests()
    {
        _currentUser = new()
        {
            Id = Guid.NewGuid()
        };
        _storyInfoProcessor = Substitute.For<IStoryInfoProcessor>();
        _currentUserContext = Substitute.For<ICurrentUserContext>();
        _currentUserContext.CurrentUser.Returns(_currentUser);

        _site = new()
        {
            Id = Guid.NewGuid(),
            ShortName = "SOMESITE",
            Address = "https://somesite.org"
        };
    }

    [TestInitialize]
    public void Initialize()
    {
        _dbContext = DbContextHelpers.CreateInMemory<BerlinDbContext>(_ => new(_));
        _dbContext.Add(_site);

        var services = new ServiceCollection();
        services.AddKeyedTransient(_site.ShortName, (_, __) => _storyInfoProcessor);

        _processor = new(_dbContext, services.BuildServiceProvider());
    }

    [TestMethod]
    public async Task ProcessAsync_CreatesPotentialStory()
    {
        var storyInfo = new StoryInfoModel
        {
            Name = "Story name",
            Summary = "The story summary"
        };
        var request = new AddPotentialStoryCommandRequest
        {
            StoryAddress = _site.Address + "/works/12345",
            SiteId = _site.Id
        };

        _storyInfoProcessor
            .ExtractStoryInfo(
                Arg.Is<string>(_ => _ == request.StoryAddress),
                Arg.Is<string>(_ => _ == _site.Address),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(storyInfo));

        var response = await _processor.ProcessAsync(request, _currentUserContext, CancellationToken.None);

        Assert.IsTrue(response.SuccessWithValue);

        var createdPotentialStory = await _dbContext.GetByIdAsync<PotentialStory>(response.Value.Id, CancellationToken.None);

        Assert.IsNotNull(createdPotentialStory);
        Assert.AreEqual(createdPotentialStory.Name, storyInfo.Name);
        Assert.AreEqual(createdPotentialStory.Summary, storyInfo.Summary);
        Assert.AreEqual(createdPotentialStory.Address, request.StoryAddress);

        Assert.AreEqual(response.Value.Id, createdPotentialStory.Id);
        Assert.AreEqual(response.Value.Name, storyInfo.Name);
        Assert.AreEqual(response.Value.Summary, storyInfo.Summary);
        Assert.AreEqual(response.Value.Address, request.StoryAddress);
    }
}
