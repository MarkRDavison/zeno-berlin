namespace mark.davison.berlin.shared.server.services.tests.Helpers;

public sealed class SiteServiceTests
{
    private readonly Mock<IStoryInfoProcessor> _storyInfoProcessor;
    private IDbContext<BerlinDbContext> _dbContext = default!;
    private SiteService _siteService = default!;
    private readonly Site _siteRegistered;
    private readonly Site _siteUnregistered;

    public SiteServiceTests()
    {
        _storyInfoProcessor = new();

        _siteRegistered = new()
        {
            Id = Guid.NewGuid(),
            ShortName = "SOMESITE",
            Address = "https://somesite.org",
            UserId = Guid.Empty,
            Created = DateTime.UtcNow,
            LastModified = DateTime.UtcNow
        };
        _siteUnregistered = new()
        {
            Id = Guid.NewGuid(),
            ShortName = "SOMESITE2",
            Address = "https://somesite2.org",
            UserId = Guid.Empty,
            Created = DateTime.UtcNow,
            LastModified = DateTime.UtcNow
        };
    }

    [Before(Test)]
    public void Initialize()
    {
        _dbContext = DbContextHelpers.CreateInMemory<BerlinDbContext>(_ => new(_));
        _dbContext.AddSync([_siteRegistered, _siteUnregistered]);

        var services = new ServiceCollection();
        services.AddKeyedTransient(_siteRegistered.ShortName, (_, __) => _storyInfoProcessor.Object);

        _siteService = new(_dbContext, services.BuildServiceProvider());
    }

    [Test]
    public async Task DetermineSiteAsync_WhereStoryAddressEmpty_ReturnsError()
    {
        var response = await _siteService.DetermineSiteAsync(
            string.Empty,
            Guid.NewGuid(),
            CancellationToken.None);

        await Assert.That(response.Valid).IsFalse();
        await Assert.That(response.Error).Contains(ValidationMessages.INVALID_PROPERTY);
        await Assert.That(response.Error).Contains(nameof(Story.Address));
    }

    [Test]
    public async Task DetermineSiteAsync_WhereStoryAddressIsNotForSupportedSite_ReturnsError()
    {
        var response = await _siteService.DetermineSiteAsync(
            "https://somesitethatdoesntexist.com/story/1234",
            null,
            CancellationToken.None);

        await Assert.That(response.Valid).IsFalse();
        await Assert.That(response.Error).Contains(ValidationMessages.UNSUPPORTED_SITE);
    }

    [Test]
    public async Task DetermineSiteAsync_WhereInvalidSiteIdGiven_ReturnsError()
    {
        var response = await _siteService.DetermineSiteAsync(
            _siteRegistered.Address + "/story/1234",
            Guid.NewGuid(),
            CancellationToken.None);

        await Assert.That(response.Valid).IsFalse();
        await Assert.That(response.Error).Contains(ValidationMessages.FAILED_TO_FIND_ENTITY);
        await Assert.That(response.Error).Contains(nameof(Site));
    }

    [Test]
    public async Task DetermineSiteAsync_WhereSiteIdGivenDoesNotMatchStoryAddress_ReturnsError()
    {
        var response = await _siteService.DetermineSiteAsync(
            "https://someothersite.net/story/1234",
            _siteRegistered.Id,
            CancellationToken.None);

        await Assert.That(response.Valid).IsFalse();
        await Assert.That(response.Error).Contains(ValidationMessages.SITE_STORY_MISMATCH);
    }

    [Test]
    public async Task DetermineSiteAsync_WhereStoryProcessorNotRegistered_ReturnsError()
    {
        var response = await _siteService.DetermineSiteAsync(
            _siteUnregistered.Address + "/story/1234",
            _siteUnregistered.Id,
            CancellationToken.None);

        await Assert.That(response.Valid).IsFalse();
        await Assert.That(response.Error).Contains(ValidationMessages.UNSUPPORTED_SITE);
    }

    [Test]
    public async Task DetermineSiteAsync_WhereExtractingExternalIdFails_ReturnsError()
    {
        _storyInfoProcessor
            .Setup(_ => _
                .ExtractExternalStoryId(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
            .Returns(string.Empty);

        var response = await _siteService.DetermineSiteAsync(
            _siteRegistered.Address + "/story",
            _siteRegistered.Id,
            CancellationToken.None);

        await Assert.That(response.Valid).IsFalse();
        await Assert.That(response.Error).Contains(ValidationMessages.INVALID_PROPERTY);
        await Assert.That(response.Error).Contains(nameof(Story.Address));
    }

    [Test]
    public async Task DetermineSiteAsync_WhereSuccess_ReturnsNoError()
    {
        _storyInfoProcessor
            .Setup(_ => _
                .ExtractExternalStoryId(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
            .Returns("1234");

        var response = await _siteService.DetermineSiteAsync(
            _siteRegistered.Address + "/story/1234",
            _siteRegistered.Id,
            CancellationToken.None);

        await Assert.That(response.Valid).IsTrue();
        await Assert.That(response.Error).IsNullOrEmpty();
        await Assert.That(response.Site).IsNotNull();
    }
}