namespace mark.davison.berlin.shared.commands.test.Scenarios.AddStory;

[TestClass]
public sealed class AddStoryCommandValidatorTests
{
    private readonly ICurrentUserContext _currentUserContext;
    private readonly ISiteService _siteService;
    private readonly User _currentUser;

    private IDbContext<BerlinDbContext> _dbContext = default!;
    private AddStoryCommandValidator _validator = default!;

    private readonly Site _site;

    public AddStoryCommandValidatorTests()
    {
        _currentUser = new()
        {
            Id = Guid.NewGuid()
        };
        _currentUserContext = Substitute.For<ICurrentUserContext>();
        _currentUserContext.CurrentUser.Returns(_currentUser);
        _siteService = Substitute.For<ISiteService>();

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

        _validator = new(_dbContext, _siteService);
    }

    [TestMethod]
    public async Task ValidateAsync_WhereStoryInfoExternalIdExists_ReturnsError()
    {
        const string externalId = "1234";

        _dbContext.AddSync(new Story
        {
            Id = Guid.NewGuid(),
            UserId = _currentUser.Id,
            ExternalId = externalId
        });

        _siteService
            .DetermineSiteAsync(Arg.Any<string>(), Arg.Any<Guid?>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new SiteInfo
            {
                Site = _site,
                ExternalId = externalId
            }));

        var response = await _validator.ValidateAsync(new()
        {
            StoryAddress = _site.Address + "/story/" + externalId
        }, _currentUserContext, CancellationToken.None);

        Assert.IsFalse(response.Success);
        Assert.IsTrue(response.Errors.Any(_ =>
            _.Contains(ValidationMessages.DUPLICATE_ENTITY) &&
            _.Contains(nameof(Story))));
    }

    [TestMethod]
    public async Task ValidateAsync_WhereStoryProcessorRegistered_ReturnsSuccess_UpdatesRequestStoryId()
    {
        const string externalId = "1234";

        var request = new AddStoryCommandRequest
        {
            SiteId = _site.Id,
            StoryAddress = _site.Address + "/story/" + externalId
        };

        _siteService
            .DetermineSiteAsync(Arg.Any<string>(), Arg.Any<Guid?>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new SiteInfo
            {
                Site = _site,
                ExternalId = externalId
            }));

        var response = await _validator.ValidateAsync(request, _currentUserContext, CancellationToken.None);

        Assert.IsTrue(response.Success);
        Assert.AreEqual(_site.Id, request.SiteId);
    }
}
