namespace mark.davison.berlin.api.commands.tests.Scenarios.AddPotentialStory;

public sealed class AddPotentialStoryCommandProcessorTests
{
    private Mock<ICurrentUserContext> _currentUserContext = default!;
    private Mock<IStoryInfoProcessor> _storyInfoProcessor = default!;
    private Mock<IDateService> _dateServiceMock;

    private readonly Guid _currentUserId = Guid.NewGuid();
    private Site _site = default!;

    private IDbContext<BerlinDbContext> _dbContext = default!;
    private AddPotentialStoryCommandProcessor _processor = default!;

    public AddPotentialStoryCommandProcessorTests()
    {
        _dateServiceMock = new();
        _dateServiceMock.Setup(_ => _.Now).Returns(DateTime.UtcNow);
    }

    [Before(Test)]
    public void BeforeTest()
    {
        _currentUserContext = new();
        _currentUserContext
            .Setup(_ => _.UserId)
            .Returns(_currentUserId);

        _storyInfoProcessor = new();

        _site = new()
        {
            Id = Guid.NewGuid(),
            UserId = Guid.Empty,
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

        _processor = new(
            _dbContext,
            _dateServiceMock.Object,
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
    public async Task ProcessAsync_CreatesPotentialStory()
    {
        var storyInfo = new StoryInfoModel
        {
            Name = "Story name",
            Summary = "The story summary"
        };

        var request = new AddPotentialStoryCommandRequest
        {
            StoryAddress = $"{_site.Address}/works/12345",
            SiteId = _site.Id
        };

        _storyInfoProcessor
            .Setup(_ => _.ExtractStoryInfo(
                It.Is<string>(_ => _ == request.StoryAddress),
                It.Is<string>(_ => _ == _site.Address),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(storyInfo);

        var response = await _processor.ProcessAsync(
            request,
            _currentUserContext.Object,
            CancellationToken.None);

        await Assert.That(response.SuccessWithValue).IsTrue();

        var createdPotentialStory =
            await _dbContext.GetByIdAsync<PotentialStory>(
                response.Value!.Id,
                CancellationToken.None);

        await Assert.That(createdPotentialStory).IsNotNull();
        await Assert.That(createdPotentialStory!.Name).IsEqualTo(storyInfo.Name);
        await Assert.That(createdPotentialStory.Summary).IsEqualTo(storyInfo.Summary);
        await Assert.That(createdPotentialStory.Address).IsEqualTo(request.StoryAddress);
        await Assert.That(response.Value.Id).IsEqualTo(createdPotentialStory.Id);
        await Assert.That(response.Value.Name).IsEqualTo(storyInfo.Name);
        await Assert.That(response.Value.Summary).IsEqualTo(storyInfo.Summary);
        await Assert.That(response.Value.Address).IsEqualTo(request.StoryAddress);
    }
}
