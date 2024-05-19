namespace mark.davison.berlin.shared.commands.test.Scenarios.Import;

[TestClass]
public sealed class ImportCommandValidatorTests
{
    private readonly ICurrentUserContext _currentUserContext;
    private readonly Guid _userId;

    private IDbContext<BerlinDbContext> _dbContext = default!;
    private ImportCommandValidator _validator = default!;

    public ImportCommandValidatorTests()
    {
        _userId = Guid.NewGuid();
        _currentUserContext = Substitute.For<ICurrentUserContext>();

        _currentUserContext.CurrentUser.Returns(new User { Id = _userId });
    }

    [TestInitialize]
    public void Initialize()
    {
        _dbContext = DbContextHelpers.CreateInMemory<BerlinDbContext>(_ => new(_));
        _validator = new(_dbContext);
    }

    [TestMethod]
    public async Task ValidateAsync_WhereStoriesAlreadyExist_ReturnsWarningAndRemovesDuplicatesFromRequest()
    {
        var existingStories = new List<Story>
        {
            new()
            {
                Id = Guid.NewGuid(),
                UserId = _userId,
                Address = "address1"
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = _userId,
                Address = "address2"
            },
        };

        _dbContext.Add(existingStories);

        var request = new ImportCommandRequest
        {
            Data = new()
            {
                Stories =
                [
                    new(){ StoryAddress = existingStories[0].Address },
                    new(){ StoryAddress = existingStories[1].Address },
                    new(){ StoryAddress = "address3" }
                ]
            }
        };

        var response = await _validator.ValidateAsync(request, _currentUserContext, CancellationToken.None);

        Assert.IsTrue(response.Success);
        Assert.IsTrue(response.Warnings.Any(_ =>
            _.Contains(ValidationMessages.DUPLICATE_ENTITY) &&
            _.Contains(existingStories.Count.ToString())));

        Assert.AreEqual(1, request.Data.Stories.Count);
    }
}
