using mark.davison.berlin.shared.models.dtos.Scenarios.Commands.AddPotentialStory;

namespace mark.davison.berlin.shared.commands.test.Scenarios.AddPotentialStory;

[TestClass]
public sealed class AddPotentialStoryCommandValidatorTests
{
    private readonly ICurrentUserContext _currentUserContext;
    private readonly ISiteService _siteService;
    private readonly User _currentUser;

    private IDbContext<BerlinDbContext> _dbContext = default!;
    private AddPotentialStoryCommandValidator _validator = default!;

    private readonly Site _site;

    public AddPotentialStoryCommandValidatorTests()
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
    public async Task ValdiateAsync_WhereSiteIsNotValid_ReturnsError()
    {
        var externalId = "1234";

        var request = new AddPotentialStoryCommandRequest
        {
            StoryAddress = _site.Address + "/works/" + externalId
        };

        _siteService
            .DetermineSiteAsync(Arg.Any<string>(), Arg.Any<Guid?>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new SiteInfo
            {
                Site = _site,
                ExternalId = externalId,
                UpdatedAddress = request.StoryAddress,
                Error = "DETERMINE_SITE_ERROR"
            }));

        var response = await _validator.ValidateAsync(request, _currentUserContext, CancellationToken.None);

        Assert.IsFalse(response.Success);
        Assert.IsTrue(response.Errors.Contains("DETERMINE_SITE_ERROR"));
    }

    [TestMethod]
    public async Task ValdiateAsync_WhereDuplicateStoryExternalId_ReturnsError()
    {
        var externalId = "1234";

        var request = new AddPotentialStoryCommandRequest
        {
            StoryAddress = _site.Address + "/works/" + externalId
        };

        _siteService
            .DetermineSiteAsync(Arg.Any<string>(), Arg.Any<Guid?>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new SiteInfo
            {
                Site = _site,
                ExternalId = externalId,
                UpdatedAddress = request.StoryAddress
            }));

        await _dbContext.AddAsync(new Story
        {
            Id = Guid.NewGuid(),
            UserId = _currentUser.Id,
            ExternalId = externalId
        }, CancellationToken.None);
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        var response = await _validator.ValidateAsync(request, _currentUserContext, CancellationToken.None);

        Assert.IsFalse(response.Success);
        Assert.IsTrue(response.Errors.Contains(
            ValidationMessages.FormatMessageParameters(
                ValidationMessages.DUPLICATE_ENTITY,
                nameof(Story),
                nameof(Story.Address))));
    }

    [TestMethod]
    public async Task ValdiateAsync_WhereDuplicatePotentialStoryAddress_ReturnsError()
    {
        var externalId = "1234";

        var request = new AddPotentialStoryCommandRequest
        {
            StoryAddress = _site.Address + "/works/" + externalId
        };

        _siteService
            .DetermineSiteAsync(Arg.Any<string>(), Arg.Any<Guid?>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new SiteInfo
            {
                Site = _site,
                ExternalId = externalId,
                UpdatedAddress = request.StoryAddress
            }));

        await _dbContext.AddAsync(new PotentialStory
        {
            Id = Guid.NewGuid(),
            UserId = _currentUser.Id,
            Address = request.StoryAddress
        }, CancellationToken.None);
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        var response = await _validator.ValidateAsync(request, _currentUserContext, CancellationToken.None);

        Assert.IsFalse(response.Success);
        Assert.IsTrue(response.Errors.Contains(
            ValidationMessages.FormatMessageParameters(
                ValidationMessages.DUPLICATE_ENTITY,
                nameof(PotentialStory),
                nameof(PotentialStory.Address))));
    }

    [TestMethod]
    public async Task ValdiateAsync_WhereNoDuplicates_ReturnsSuccess()
    {
        var externalId = "1234";

        var request = new AddPotentialStoryCommandRequest
        {
            StoryAddress = _site.Address + "/works/" + externalId
        };

        _siteService
            .DetermineSiteAsync(Arg.Any<string>(), Arg.Any<Guid?>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new SiteInfo
            {
                Site = _site,
                ExternalId = externalId,
                UpdatedAddress = request.StoryAddress
            }));

        var response = await _validator.ValidateAsync(request, _currentUserContext, CancellationToken.None);

        Assert.IsTrue(response.Success);
    }
}
