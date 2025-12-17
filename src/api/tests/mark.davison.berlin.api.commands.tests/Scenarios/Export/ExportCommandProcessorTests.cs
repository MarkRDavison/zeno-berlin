namespace mark.davison.berlin.api.commands.tests.Scenarios.Export;

public sealed class ExportCommandProcessorTests
{
    private readonly Guid _currentUserId = Guid.NewGuid();

    private Mock<ICurrentUserContext> _currentUserContext = default!;
    private IDbContext<BerlinDbContext> _dbContext = default!;
    private ExportCommandProcessor _processor = default!;

    [Before(Test)]
    public void BeforeTest()
    {
        _currentUserContext = new();
        _currentUserContext.Setup(_ => _.UserId).Returns(_currentUserId);

        _dbContext = DbContextHelpers.CreateInMemory<BerlinDbContext>(_ => new(_));
        _processor = new(_dbContext);
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
    public async Task ProcessAsync_ExportsStoriesAndUpdates_WithCorrectEntitiesSelected()
    {
        var currentUserStories = new List<Story>
        {
            new(){ Id = Guid.NewGuid(), UserId = _currentUserId, Created = DateTime.UtcNow, LastModified = DateTime.UtcNow },
            new(){ Id = Guid.NewGuid(), UserId = _currentUserId, Created = DateTime.UtcNow, LastModified = DateTime.UtcNow },
            new(){ Id = Guid.NewGuid(), UserId = _currentUserId, Created = DateTime.UtcNow, LastModified = DateTime.UtcNow },
            new(){ Id = Guid.NewGuid(), UserId = _currentUserId, Created = DateTime.UtcNow, LastModified = DateTime.UtcNow }
        };

        var otherUserStories = new List<Story>
        {
            new(){ Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Created = DateTime.UtcNow, LastModified = DateTime.UtcNow },
            new(){ Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Created = DateTime.UtcNow, LastModified = DateTime.UtcNow },
            new(){ Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Created = DateTime.UtcNow, LastModified = DateTime.UtcNow },
            new(){ Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Created = DateTime.UtcNow, LastModified = DateTime.UtcNow }
        };

        var currentUserStoryUpdates = new List<StoryUpdate>
        {
            new(){ Id = Guid.NewGuid(), UserId = _currentUserId, StoryId = currentUserStories[0].Id, Created = DateTime.UtcNow, LastModified = DateTime.UtcNow },
            new(){ Id = Guid.NewGuid(), UserId = _currentUserId, StoryId = currentUserStories[0].Id, Created = DateTime.UtcNow, LastModified = DateTime.UtcNow },
            new(){ Id = Guid.NewGuid(), UserId = _currentUserId, StoryId = currentUserStories[1].Id, Created = DateTime.UtcNow, LastModified = DateTime.UtcNow },
            new(){ Id = Guid.NewGuid(), UserId = _currentUserId, StoryId = currentUserStories[1].Id, Created = DateTime.UtcNow, LastModified = DateTime.UtcNow },
            new(){ Id = Guid.NewGuid(), UserId = _currentUserId, StoryId = currentUserStories[2].Id, Created = DateTime.UtcNow, LastModified = DateTime.UtcNow },
            new(){ Id = Guid.NewGuid(), UserId = _currentUserId, StoryId = currentUserStories[3].Id, Created = DateTime.UtcNow, LastModified = DateTime.UtcNow },
            new(){ Id = Guid.NewGuid(), UserId = _currentUserId, StoryId = currentUserStories[3].Id, Created = DateTime.UtcNow, LastModified = DateTime.UtcNow },
            new(){ Id = Guid.NewGuid(), UserId = _currentUserId, StoryId = currentUserStories[3].Id, Created = DateTime.UtcNow, LastModified = DateTime.UtcNow }
        };

        var otherUserStoryUpdates = new List<StoryUpdate>
        {
            new(){ Id = Guid.NewGuid(), UserId = Guid.NewGuid(), StoryId = otherUserStories[0].Id, Created = DateTime.UtcNow, LastModified = DateTime.UtcNow },
            new(){ Id = Guid.NewGuid(), UserId = Guid.NewGuid(), StoryId = otherUserStories[0].Id, Created = DateTime.UtcNow, LastModified = DateTime.UtcNow },
            new(){ Id = Guid.NewGuid(), UserId = Guid.NewGuid(), StoryId = otherUserStories[1].Id, Created = DateTime.UtcNow, LastModified = DateTime.UtcNow },
            new(){ Id = Guid.NewGuid(), UserId = Guid.NewGuid(), StoryId = otherUserStories[1].Id, Created = DateTime.UtcNow, LastModified = DateTime.UtcNow },
            new(){ Id = Guid.NewGuid(), UserId = Guid.NewGuid(), StoryId = otherUserStories[2].Id, Created = DateTime.UtcNow, LastModified = DateTime.UtcNow },
            new(){ Id = Guid.NewGuid(), UserId = Guid.NewGuid(), StoryId = otherUserStories[3].Id, Created = DateTime.UtcNow, LastModified = DateTime.UtcNow },
            new(){ Id = Guid.NewGuid(), UserId = Guid.NewGuid(), StoryId = otherUserStories[3].Id, Created = DateTime.UtcNow, LastModified = DateTime.UtcNow },
            new(){ Id = Guid.NewGuid(), UserId = Guid.NewGuid(), StoryId = otherUserStories[3].Id, Created = DateTime.UtcNow, LastModified = DateTime.UtcNow }
        };

        _dbContext.AddSync(currentUserStories);
        _dbContext.AddSync(otherUserStories);
        _dbContext.AddSync(currentUserStoryUpdates);
        _dbContext.AddSync(otherUserStoryUpdates);

        var request = new ExportCommandRequest();

        var response = await _processor.ProcessAsync(request, _currentUserContext.Object, CancellationToken.None);

        await Assert.That(response.SuccessWithValue).IsTrue();

        var storyCount = response.Value.Stories.Count;
        var storyUpdateCount = response.Value.Stories.SelectMany(_ => _.Updates).Count();

        await Assert.That(storyCount).IsEqualTo(currentUserStories.Count);
        await Assert.That(storyUpdateCount).IsEqualTo(currentUserStoryUpdates.Count);
    }

    [Test]
    public async Task ProcessAsync_ExportsStoriesAndUpdates_WithCorrectValues()
    {
        var story = new Story
        {
            Id = Guid.NewGuid(),
            UserId = _currentUserId,
            Address = "https://the.story.address.com/works/123",
            Favourite = true,
            Created = DateTime.UtcNow,
            LastModified = DateTime.UtcNow
        };

        var update = new StoryUpdate
        {
            Id = Guid.NewGuid(),
            UserId = _currentUserId,
            StoryId = story.Id,
            Complete = true,
            CurrentChapters = 123,
            TotalChapters = null,
            LastAuthored = DateOnly.FromDateTime(DateTime.Now),
            Created = DateTime.UtcNow,
            LastModified = DateTime.UtcNow
        };

        _dbContext.AddSync(story);
        _dbContext.AddSync(update);

        var request = new ExportCommandRequest();

        var response = await _processor.ProcessAsync(request, _currentUserContext.Object, CancellationToken.None);

        await Assert.That(response.SuccessWithValue).IsTrue();

        var exportedStory = response.Value.Stories.First();
        var exportedUpdate = exportedStory.Updates.First();

        await Assert.That(exportedStory.Favourite).IsEqualTo(story.Favourite);
        await Assert.That(exportedStory.StoryAddress).IsEqualTo(story.Address);

        await Assert.That(exportedUpdate.Complete).IsEqualTo(update.Complete);
        await Assert.That(exportedUpdate.CurrentChapters).IsEqualTo(update.CurrentChapters);
        await Assert.That(exportedUpdate.LastAuthored).IsEqualTo(update.LastAuthored);
    }
}