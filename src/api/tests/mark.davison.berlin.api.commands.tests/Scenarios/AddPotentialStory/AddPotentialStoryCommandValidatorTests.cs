namespace mark.davison.berlin.api.commands.tests.Scenarios.AddPotentialStory;

public sealed class AddPotentialStoryCommandValidatorTests
{
    private readonly Mock<ICurrentUserContext> _currentUserContext = new();
    private readonly Mock<ISiteService> _siteService = new();

    private readonly Guid _currentUserId = Guid.NewGuid();
    private readonly Site _site;

    private IDbContext<BerlinDbContext> _dbContext = default!;
    private AddPotentialStoryCommandValidator _validator = default!;

    public AddPotentialStoryCommandValidatorTests()
    {
        _currentUserContext
            .Setup(_ => _.UserId)
            .Returns(_currentUserId);

        _site = new()
        {
            Id = Guid.NewGuid(),
            ShortName = "SOMESITE",
            Address = "https://somesite.org",
            Created = DateTime.UtcNow,
            LastModified = DateTime.UtcNow,
            UserId = Guid.Empty
        };
    }

    [Before(Test)]
    public void Initialize()
    {
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
    public async Task ValidateAsync_WhereSiteIsNotValid_ReturnsError()
    {
        Initialize();

        var externalId = "1234";

        var request = new AddPotentialStoryCommandRequest
        {
            StoryAddress = $"{_site.Address}/works/{externalId}"
        };

        _siteService
            .Setup(_ => _.DetermineSiteAsync(
                It.IsAny<string>(),
                It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SiteInfo
            {
                Site = _site,
                ExternalId = externalId,
                UpdatedAddress = request.StoryAddress,
                Error = "DETERMINE_SITE_ERROR"
            });

        var response = await _validator.ValidateAsync(
            request,
            _currentUserContext.Object,
            CancellationToken.None);

        await Assert.That(response.Success).IsFalse();
        await Assert.That(response.Errors).Contains("DETERMINE_SITE_ERROR");
    }

    [Test]
    public async Task ValidateAsync_WhereDuplicateStoryExternalId_ReturnsError()
    {
        Initialize();

        var externalId = "1234";

        var request = new AddPotentialStoryCommandRequest
        {
            StoryAddress = $"{_site.Address}/works/{externalId}"
        };

        _siteService
            .Setup(_ => _.DetermineSiteAsync(
                It.IsAny<string>(),
                It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SiteInfo
            {
                Site = _site,
                ExternalId = externalId,
                UpdatedAddress = request.StoryAddress
            });

        await _dbContext.AddAsync(new Story
        {
            Id = Guid.NewGuid(),
            UserId = _currentUserId,
            ExternalId = externalId,
            Created = DateTime.UtcNow,
            LastModified = DateTime.UtcNow
        }, CancellationToken.None);

        await _dbContext.SaveChangesAsync(CancellationToken.None);

        var response = await _validator.ValidateAsync(
            request,
            _currentUserContext.Object,
            CancellationToken.None);

        await Assert.That(response.Success).IsFalse();
        await Assert.That(response.Errors).Contains(
            ValidationMessages.FormatMessageParameters(
                ValidationMessages.DUPLICATE_ENTITY,
                nameof(Story),
                nameof(Story.Address)));
    }

    [Test]
    public async Task ValidateAsync_WhereDuplicatePotentialStoryAddress_ReturnsError()
    {
        Initialize();

        var externalId = "1234";

        var request = new AddPotentialStoryCommandRequest
        {
            StoryAddress = $"{_site.Address}/works/{externalId}"
        };

        _siteService
            .Setup(_ => _.DetermineSiteAsync(
                It.IsAny<string>(),
                It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SiteInfo
            {
                Site = _site,
                ExternalId = externalId,
                UpdatedAddress = request.StoryAddress
            });

        await _dbContext.AddAsync(new PotentialStory
        {
            Id = Guid.NewGuid(),
            UserId = _currentUserId,
            Address = request.StoryAddress,
            Created = DateTime.UtcNow,
            LastModified = DateTime.UtcNow
        }, CancellationToken.None);

        await _dbContext.SaveChangesAsync(CancellationToken.None);

        var response = await _validator.ValidateAsync(
            request,
            _currentUserContext.Object,
            CancellationToken.None);

        await Assert.That(response.Success).IsFalse();
        await Assert.That(response.Errors).Contains(
            ValidationMessages.FormatMessageParameters(
                ValidationMessages.DUPLICATE_ENTITY,
                nameof(PotentialStory),
                nameof(PotentialStory.Address)));
    }

    [Test]
    public async Task ValidateAsync_WhereNoDuplicates_ReturnsSuccess()
    {
        Initialize();

        var externalId = "1234";

        var request = new AddPotentialStoryCommandRequest
        {
            StoryAddress = $"{_site.Address}/works/{externalId}"
        };

        _siteService
            .Setup(_ => _.DetermineSiteAsync(
                It.IsAny<string>(),
                It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SiteInfo
            {
                Site = _site,
                ExternalId = externalId,
                UpdatedAddress = request.StoryAddress
            });

        var response = await _validator.ValidateAsync(
            request,
            _currentUserContext.Object,
            CancellationToken.None);

        await Assert.That(response.Success).IsTrue();
    }
}