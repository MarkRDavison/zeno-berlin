namespace mark.davison.berlin.api.commands.tests.Scenarios.Import;

public sealed class ImportCommandValidatorTests
{
    private readonly Guid _currentUserId = Guid.NewGuid();

    private Mock<ICurrentUserContext> _currentUserContext = default!;
    private IDbContext<BerlinDbContext> _dbContext = default!;
    private ImportCommandValidator _validator = default!;

    [Before(Test)]
    public void BeforeTest()
    {
        _currentUserContext = new();
        _currentUserContext.Setup(_ => _.UserId).Returns(_currentUserId);

        _dbContext = DbContextHelpers.CreateInMemory<BerlinDbContext>(_ => new(_));
        _validator = new(_dbContext);
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
    public async Task ValidateAsync_WhereStoriesAlreadyExist_ReturnsWarningAndRemovesDuplicatesFromRequest()
    {
        var existingStories = new List<Story>
        {
            new() { Id = Guid.NewGuid(), UserId = _currentUserId, Address = "address1", Created = DateTime.UtcNow, LastModified = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), UserId = _currentUserId, Address = "address2", Created = DateTime.UtcNow, LastModified = DateTime.UtcNow },
        };

        _dbContext.AddSync(existingStories);

        var request = new ImportCommandRequest
        {
            Data = new()
            {
                Stories =
                [
                    new() { StoryAddress = existingStories[0].Address },
                    new() { StoryAddress = existingStories[1].Address },
                    new() { StoryAddress = "address3" }
                ]
            }
        };

        var response = await _validator.ValidateAsync(request, _currentUserContext.Object, CancellationToken.None);

        await Assert.That(response.Success).IsTrue();
        await Assert.That(response.Warnings.Any(_ => _.Contains(ValidationMessages.DUPLICATE_ENTITY) && _.Contains(existingStories.Count.ToString()))).IsTrue();
        await Assert.That(request.Data.Stories.Count).IsEqualTo(1);
    }
}