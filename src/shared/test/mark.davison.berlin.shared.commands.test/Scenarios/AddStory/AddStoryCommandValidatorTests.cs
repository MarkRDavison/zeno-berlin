namespace mark.davison.berlin.shared.commands.test.Scenarios.AddStory;

[TestClass]
public sealed class AddStoryCommandValidatorTests
{
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IStoryInfoProcessor _storyInfoProcessor;
    private readonly User _currentUser;

    private IDbContext<BerlinDbContext> _dbContext = default!;
    private AddStoryCommandValidator _validator = default!;

    private readonly Site _siteRegistered;
    private readonly Site _siteUnregistered;

    public AddStoryCommandValidatorTests()
    {
        _currentUser = new()
        {
            Id = Guid.NewGuid()
        };
        _currentUserContext = Substitute.For<ICurrentUserContext>();
        _currentUserContext.CurrentUser.Returns(_currentUser);
        _storyInfoProcessor = Substitute.For<IStoryInfoProcessor>();

        _siteRegistered = new()
        {
            Id = Guid.NewGuid(),
            ShortName = "SOMESITE",
            Address = "https://somesite.org"
        };
        _siteUnregistered = new()
        {
            Id = Guid.NewGuid(),
            ShortName = "SOMESITE2",
            Address = "https://somesite2.org"
        };

        _storyInfoProcessor
            .ExtractExternalStoryId(
                Arg.Any<string>(),
                Arg.Any<string>())
            .Returns("1234");
    }

    [TestInitialize]
    public void Initialize()
    {
        _dbContext = DbContextHelpers.CreateInMemory<BerlinDbContext>(_ => new(_));
        _dbContext.Add([_siteRegistered, _siteUnregistered]);

        var services = new ServiceCollection();
        services.AddKeyedTransient(_siteRegistered.ShortName, (_, __) => _storyInfoProcessor);

        _validator = new(_dbContext, services.BuildServiceProvider());
    }

    [TestMethod]
    public async Task ValidateAsync_WhereStoryAddressEmpty_ReturnsError()
    {
        var response = await _validator.ValidateAsync(new()
        {
            SiteId = Guid.NewGuid()
        }, _currentUserContext, CancellationToken.None);

        Assert.IsFalse(response.Success);
        Assert.IsTrue(response.Errors.Any(_ =>
            _.Contains(ValidationMessages.INVALID_PROPERTY) &&
            _.Contains(nameof(AddStoryCommandRequest.StoryAddress))));
    }

    [TestMethod]
    public async Task ValidateAsync_WhereStoryAddressIsNotForSupportedSite_ReturnsError()
    {
        var response = await _validator.ValidateAsync(new()
        {
            StoryAddress = "https://somesitethatdoesntexist.com/story/1234"
        }, _currentUserContext, CancellationToken.None);

        Assert.IsFalse(response.Success);
        Assert.IsTrue(response.Errors.Any(_ =>
            _.Contains(ValidationMessages.UNSUPPORTED_SITE)));
    }

    [TestMethod]
    public async Task ValidateAsync_WhereInvalidSiteIdGiven_ReturnsError()
    {
        var response = await _validator.ValidateAsync(new()
        {
            SiteId = Guid.NewGuid(),
            StoryAddress = _siteRegistered.Address + "/story/1234"
        }, _currentUserContext, CancellationToken.None);

        Assert.IsFalse(response.Success);
        Assert.IsTrue(response.Errors.Any(_ =>
            _.Contains(ValidationMessages.FAILED_TO_FIND_ENTITY) &&
            _.Contains(nameof(Site))));
    }

    [TestMethod]
    public async Task ValidateAsync_WhereSiteIdGivenDoesNotMatchStoryAddress_ReturnsError()
    {
        var response = await _validator.ValidateAsync(new()
        {
            SiteId = _siteRegistered.Id,
            StoryAddress = "https://someothersite.net/story/1234"
        }, _currentUserContext, CancellationToken.None);

        Assert.IsFalse(response.Success);
        Assert.IsTrue(response.Errors.Any(_ =>
            _.Contains(ValidationMessages.SITE_STORY_MISMATCH)));
    }

    [TestMethod]
    public async Task ValidateAsync_WhereStoryProcessorNotRegistered_ReturnsError()
    {
        var response = await _validator.ValidateAsync(new()
        {
            StoryAddress = _siteUnregistered.Address + "/story/1234"
        }, _currentUserContext, CancellationToken.None);

        Assert.IsFalse(response.Success);
        Assert.IsTrue(response.Errors.Any(_ =>
            _.Contains(ValidationMessages.UNSUPPORTED_SITE)));
    }

    [TestMethod]
    public async Task ValidateAsync_WhereExtractingExternalIdFails_ReturnsError()
    {
        _storyInfoProcessor
            .ExtractExternalStoryId(
                Arg.Any<string>(),
                Arg.Any<string>())
            .Returns(string.Empty);

        var response = await _validator.ValidateAsync(new()
        {
            StoryAddress = _siteRegistered.Address + "/story"
        }, _currentUserContext, CancellationToken.None);

        Assert.IsFalse(response.Success);
        Assert.IsTrue(response.Errors.Any(_ =>
            _.Contains(ValidationMessages.INVALID_PROPERTY) &&
            _.Contains(nameof(AddStoryCommandRequest.StoryAddress))));
    }

    [TestMethod]
    public async Task ValidateAsync_WhereStoryInfoExternalIdExists_ReturnsError()
    {
        const string externalId = "1234";

        _dbContext.Add(new Story
        {
            Id = Guid.NewGuid(),
            UserId = _currentUser.Id,
            ExternalId = externalId
        });

        _storyInfoProcessor
            .ExtractExternalStoryId(
                Arg.Any<string>(),
                Arg.Any<string>())
            .Returns(externalId);

        var response = await _validator.ValidateAsync(new()
        {
            StoryAddress = _siteRegistered.Address + "/story/" + externalId
        }, _currentUserContext, CancellationToken.None);

        Assert.IsFalse(response.Success);
        Assert.IsTrue(response.Errors.Any(_ =>
            _.Contains(ValidationMessages.DUPLICATE_ENTITY) &&
            _.Contains(nameof(Story))));
    }

    [TestMethod]
    public async Task ValidateAsync_WhereStoryProcessorRegistered_ReturnsSuccess_UpdatesRequestStoryId()
    {
        var request = new AddStoryCommandRequest
        {
            SiteId = _siteRegistered.Id,
            StoryAddress = _siteRegistered.Address + "/story/1234"
        };

        var response = await _validator.ValidateAsync(request, _currentUserContext, CancellationToken.None);

        Assert.IsTrue(response.Success);
        Assert.AreEqual(_siteRegistered.Id, request.SiteId);
    }
}
