namespace mark.davison.shared.services.test.Helpers;


[TestClass]
public sealed class SiteServiceTests
{
    private readonly IStoryInfoProcessor _storyInfoProcessor;
    private IDbContext<BerlinDbContext> _dbContext = default!;
    private SiteService _siteService = default!;
    private readonly Site _siteRegistered;
    private readonly Site _siteUnregistered;

    public SiteServiceTests()
    {
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
    }

    [TestInitialize]
    public void Initialize()
    {
        _dbContext = DbContextHelpers.CreateInMemory<BerlinDbContext>(_ => new(_));
        _dbContext.AddSync([_siteRegistered, _siteUnregistered]);

        var services = new ServiceCollection();
        services.AddKeyedTransient(_siteRegistered.ShortName, (_, __) => _storyInfoProcessor);

        _siteService = new(_dbContext, services.BuildServiceProvider());
    }

    [TestMethod]
    public async Task DetermineSiteAsync_WhereStoryAddressEmpty_ReturnsError()
    {
        var response = await _siteService.DetermineSiteAsync(
            string.Empty,
            Guid.NewGuid(),
            CancellationToken.None);

        Assert.IsFalse(response.Valid);
        Assert.IsTrue(response.Error.Contains(ValidationMessages.INVALID_PROPERTY));
        Assert.IsTrue(response.Error.Contains(nameof(Story.Address)));
    }

    [TestMethod]
    public async Task DetermineSiteAsync_WhereStoryAddressIsNotForSupportedSite_ReturnsError()
    {
        var response = await _siteService.DetermineSiteAsync(
            "https://somesitethatdoesntexist.com/story/1234",
            null,
            CancellationToken.None);

        Assert.IsFalse(response.Valid);
        Assert.IsTrue(response.Error.Contains(ValidationMessages.UNSUPPORTED_SITE));
    }

    [TestMethod]
    public async Task DetermineSiteAsync_WhereInvalidSiteIdGiven_ReturnsError()
    {
        var response = await _siteService.DetermineSiteAsync(
            _siteRegistered.Address + "/story/1234",
            Guid.NewGuid(),
            CancellationToken.None);

        Assert.IsFalse(response.Valid);
        Assert.IsTrue(response.Error.Contains(ValidationMessages.FAILED_TO_FIND_ENTITY));
        Assert.IsTrue(response.Error.Contains(nameof(Site)));
    }

    [TestMethod]
    public async Task DetermineSiteAsync_WhereSiteIdGivenDoesNotMatchStoryAddress_ReturnsError()
    {
        var response = await _siteService.DetermineSiteAsync(
            "https://someothersite.net/story/1234",
            _siteRegistered.Id,
            CancellationToken.None);

        Assert.IsFalse(response.Valid);
        Assert.IsTrue(response.Error.Contains(ValidationMessages.SITE_STORY_MISMATCH));
    }

    [TestMethod]
    public async Task DetermineSiteAsync_WhereStoryProcessorNotRegistered_ReturnsError()
    {
        var response = await _siteService.DetermineSiteAsync(
            _siteUnregistered.Address + "/story/1234",
            _siteUnregistered.Id,
            CancellationToken.None);

        Assert.IsFalse(response.Valid);
        Assert.IsTrue(response.Error.Contains(ValidationMessages.UNSUPPORTED_SITE));
    }

    [TestMethod]
    public async Task DetermineSiteAsync_WhereExtractingExternalIdFails_ReturnsError()
    {
        _storyInfoProcessor
            .ExtractExternalStoryId(
                Arg.Any<string>(),
                Arg.Any<string>())
            .Returns(string.Empty);

        var response = await _siteService.DetermineSiteAsync(
            _siteRegistered.Address + "/story",
            _siteRegistered.Id,
            CancellationToken.None);

        Assert.IsFalse(response.Valid);
        Assert.IsTrue(response.Error.Contains(ValidationMessages.INVALID_PROPERTY));
        Assert.IsTrue(response.Error.Contains(nameof(Story.Address)));
    }

    [TestMethod]
    public async Task DetermineSiteAsync_WhereSuccess_ReturnsNoError()
    {
        _storyInfoProcessor
            .ExtractExternalStoryId(
                Arg.Any<string>(),
                Arg.Any<string>())
            .Returns("1234");

        var response = await _siteService.DetermineSiteAsync(
            _siteRegistered.Address + "/story/1234",
            _siteRegistered.Id,
            CancellationToken.None);

        Assert.IsTrue(response.Valid);
        Assert.IsTrue(string.IsNullOrEmpty(response.Error));
        Assert.IsNotNull(response.Site);
    }
}
