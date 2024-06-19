namespace mark.davison.berlin.shared.commands.test.Scenarios.Export;

[TestClass]
public sealed class ExportCommandProcessorTests
{
    private readonly ICurrentUserContext _currentUserContext;
    private readonly User _currentUser;

    private IDbContext<BerlinDbContext> _dbContext = default!;
    private ExportCommandProcessor _processor = default!;

    public ExportCommandProcessorTests()
    {
        _currentUser = new()
        {
            Id = Guid.NewGuid()
        };
        _currentUserContext = Substitute.For<ICurrentUserContext>();
        _currentUserContext.CurrentUser.Returns(_currentUser);
    }

    [TestInitialize]
    public void Initialize()
    {
        _dbContext = DbContextHelpers.CreateInMemory<BerlinDbContext>(_ => new(_));
        _processor = new(_dbContext);
    }

    [TestMethod]
    public async Task ProcessAsync_ExportsStoriesAndUpdates_WithCorrectEntitiesSelected()
    {
        var currentUserStories = new List<Story>
        {
            new(){ Id = Guid.NewGuid(), UserId = _currentUser.Id },
            new(){ Id = Guid.NewGuid(), UserId = _currentUser.Id },
            new(){ Id = Guid.NewGuid(), UserId = _currentUser.Id },
            new(){ Id = Guid.NewGuid(), UserId = _currentUser.Id }
        };
        var otherUserStories = new List<Story>
        {
            new(){ Id = Guid.NewGuid(), UserId = Guid.NewGuid() },
            new(){ Id = Guid.NewGuid(), UserId = Guid.NewGuid() },
            new(){ Id = Guid.NewGuid(), UserId = Guid.NewGuid() },
            new(){ Id = Guid.NewGuid(), UserId = Guid.NewGuid() }
        };

        var currentUserStoryUpdates = new List<StoryUpdate>
        {
            new(){ Id = Guid.NewGuid(), UserId = _currentUser.Id, StoryId = currentUserStories[0].Id },
            new(){ Id = Guid.NewGuid(), UserId = _currentUser.Id, StoryId = currentUserStories[0].Id },
            new(){ Id = Guid.NewGuid(), UserId = _currentUser.Id, StoryId = currentUserStories[1].Id },
            new(){ Id = Guid.NewGuid(), UserId = _currentUser.Id, StoryId = currentUserStories[1].Id },
            new(){ Id = Guid.NewGuid(), UserId = _currentUser.Id, StoryId = currentUserStories[2].Id },
            new(){ Id = Guid.NewGuid(), UserId = _currentUser.Id, StoryId = currentUserStories[3].Id },
            new(){ Id = Guid.NewGuid(), UserId = _currentUser.Id, StoryId = currentUserStories[3].Id },
            new(){ Id = Guid.NewGuid(), UserId = _currentUser.Id, StoryId = currentUserStories[3].Id }
        };
        var otherUserStoryUpdates = new List<StoryUpdate>
        {
            new(){ Id = Guid.NewGuid(), UserId = Guid.NewGuid(), StoryId = otherUserStories[0].Id },
            new(){ Id = Guid.NewGuid(), UserId = Guid.NewGuid(), StoryId = otherUserStories[0].Id },
            new(){ Id = Guid.NewGuid(), UserId = Guid.NewGuid(), StoryId = otherUserStories[1].Id },
            new(){ Id = Guid.NewGuid(), UserId = Guid.NewGuid(), StoryId = otherUserStories[1].Id },
            new(){ Id = Guid.NewGuid(), UserId = Guid.NewGuid(), StoryId = otherUserStories[2].Id },
            new(){ Id = Guid.NewGuid(), UserId = Guid.NewGuid(), StoryId = otherUserStories[3].Id },
            new(){ Id = Guid.NewGuid(), UserId = Guid.NewGuid(), StoryId = otherUserStories[3].Id },
            new(){ Id = Guid.NewGuid(), UserId = Guid.NewGuid(), StoryId = otherUserStories[3].Id }
        };

        _dbContext.AddSync(currentUserStories);
        _dbContext.AddSync(otherUserStories);
        _dbContext.AddSync(currentUserStoryUpdates);
        _dbContext.AddSync(otherUserStoryUpdates);

        var request = new ExportCommandRequest { };

        var response = await _processor.ProcessAsync(request, _currentUserContext, CancellationToken.None);

        Assert.IsTrue(response.SuccessWithValue);

        var storyCount = response.Value.Stories.Count;
        var storyUpdateCount = response.Value.Stories.SelectMany(_ => _.Updates).Count();

        Assert.AreEqual(currentUserStories.Count, storyCount);
        Assert.AreEqual(currentUserStoryUpdates.Count, storyUpdateCount);
    }


    [TestMethod]
    public async Task ProcessAsync_ExportsStoriesAndUpdates_WithCorrectValues()
    {
        var story = new Story
        {
            Id = Guid.NewGuid(),
            UserId = _currentUser.Id,
            Address = "https://the.story.address.com/works/123",
            Favourite = true
        };

        var update = new StoryUpdate
        {
            Id = Guid.NewGuid(),
            UserId = _currentUser.Id,
            StoryId = story.Id,
            Complete = true,
            CurrentChapters = 123,
            TotalChapters = null,
            LastAuthored = DateOnly.FromDateTime(DateTime.Now)
        };

        _dbContext.AddSync(story);
        _dbContext.AddSync(update);

        var request = new ExportCommandRequest { };

        var response = await _processor.ProcessAsync(request, _currentUserContext, CancellationToken.None);

        Assert.IsTrue(response.SuccessWithValue);

        var exportedStory = response.Value.Stories.First();
        var exportedUpdate = exportedStory.Updates.First();

        Assert.AreEqual(story.Favourite, exportedStory.Favourite);
        Assert.AreEqual(story.Address, exportedStory.StoryAddress);

        Assert.AreEqual(update.Complete, exportedUpdate.Complete);
        Assert.AreEqual(update.CurrentChapters, exportedUpdate.CurrentChapters);
        Assert.AreEqual(update.CurrentChapters, exportedUpdate.CurrentChapters);
        Assert.AreEqual(update.LastAuthored, exportedUpdate.LastAuthored);
    }
}
