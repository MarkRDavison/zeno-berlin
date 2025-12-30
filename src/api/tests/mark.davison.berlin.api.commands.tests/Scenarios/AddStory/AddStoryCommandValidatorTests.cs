namespace mark.davison.berlin.api.commands.tests.Scenarios.AddStory;

public sealed class AddStoryCommandValidatorTests
{
    private readonly Guid _currentUserId = Guid.NewGuid();

    private Mock<ICurrentUserContext> _currentUserContext = default!;
    private Mock<ISiteService> _siteService = default!;

    private IDbContext<BerlinDbContext> _dbContext = default!;
    private AddStoryCommandValidator _validator = default!;

    private Site _site = default!;

    [Before(Test)]
    public void BeforeTest()
    {
        _currentUserContext = new();
        _currentUserContext
            .Setup(_ => _.UserId)
            .Returns(_currentUserId);

        _siteService = new();

        _site = new()
        {
            Id = Guid.NewGuid(),
            UserId = _currentUserId,
            ShortName = "SOMESITE",
            Address = "https://somesite.org",
            Created = DateTime.UtcNow,
            LastModified = DateTime.UtcNow
        };

        _dbContext = DbContextHelpers.CreateInMemory<BerlinDbContext>(_ => new(_));

        _validator = new(_dbContext, _siteService.Object);
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
    public async Task ValidateAsync_WhereStoryInfoExternalIdExists_ReturnsError()
    {
        const string externalId = "1234";

        _dbContext.AddSync(new Story
        {
            Id = Guid.NewGuid(),
            UserId = _currentUserId,
            ExternalId = externalId,
            Created = DateTime.UtcNow,
            LastModified = DateTime.UtcNow
        });

        _siteService
            .Setup(_ => _.DetermineSiteAsync(
                It.IsAny<string>(),
                It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SiteInfo
            {
                Site = _site,
                ExternalId = externalId
            });

        var response = await _validator.ValidateAsync(
            new AddStoryCommandRequest
            {
                StoryAddress = $"{_site.Address}/story/{externalId}"
            },
            _currentUserContext.Object,
            CancellationToken.None);

        await Assert.That(response.Success).IsFalse();
        await Assert.That(response.Errors.Any(_ =>
            _.Contains(ValidationMessages.DUPLICATE_ENTITY) &&
            _.Contains(nameof(Story)))).IsTrue();
    }

    [Test]
    public async Task ValidateAsync_WhereStoryProcessorRegistered_ReturnsSuccess_UpdatesRequestStoryId()
    {
        const string externalId = "1234";

        var request = new AddStoryCommandRequest
        {
            SiteId = _site.Id,
            StoryAddress = $"{_site.Address}/story/{externalId}"
        };

        _siteService
            .Setup(_ => _.DetermineSiteAsync(
                It.IsAny<string>(),
                It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SiteInfo
            {
                Site = _site,
                ExternalId = externalId
            });

        var response = await _validator.ValidateAsync(
            request,
            _currentUserContext.Object,
            CancellationToken.None);

        await Assert.That(response.Success).IsTrue();
        await Assert.That(request.SiteId).IsEqualTo(_site.Id);
    }
}