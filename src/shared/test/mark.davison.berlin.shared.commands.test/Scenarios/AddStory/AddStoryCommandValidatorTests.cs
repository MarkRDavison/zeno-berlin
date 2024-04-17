namespace mark.davison.berlin.shared.commands.test.Scenarios.AddStory;

[TestClass]
public class AddStoryCommandValidatorTests
{
    private readonly IValidationContext _validationContext;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IStoryInfoProcessor _storyInfoProcessor;
    private readonly IRepository _repository;
    private readonly AddStoryCommandValidator _validator;

    private readonly Site _siteRegistered;
    private readonly Site _siteUnregistered;

    public AddStoryCommandValidatorTests()
    {
        _validationContext = Substitute.For<IValidationContext>();
        _currentUserContext = Substitute.For<ICurrentUserContext>();
        _storyInfoProcessor = Substitute.For<IStoryInfoProcessor>();
        _repository = Substitute.For<IRepository>();

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

        _validationContext
            .GetByProperty<Story, string>(
                Arg.Any<Expression<Func<Story, bool>>>(),
                Arg.Is<string>(nameof(Story.ExternalId)),
                Arg.Any<CancellationToken>())
            .Returns(_ => Task.FromResult<Story?>(null));

        _storyInfoProcessor
            .ExtractExternalStoryId(
                Arg.Any<string>())
            .Returns("1234");

        _validationContext
            .GetAll<Site>(Arg.Any<CancellationToken>())
            .Returns(_ => Task.FromResult(new List<Site> { _siteRegistered, _siteUnregistered }));

        _repository
            .BeginTransaction()
            .Returns(new StubAsyncDisposable());

        var services = new ServiceCollection();
        services.AddScoped(_ => _repository);
        services.AddKeyedTransient(_siteRegistered.ShortName, (_, __) => _storyInfoProcessor);

        _validator = new(_validationContext, services.BuildServiceProvider());
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
    public async Task ValidateAsync_DoesNotCheckValidationContext_ForNullSiteId()
    {
        _validationContext
            .GetById<Site>(
                Arg.Any<Guid>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Site?>(null));

        await _validator.ValidateAsync(new()
        {
            StoryAddress = _siteRegistered.Address + "/story/1234"
        }, _currentUserContext, CancellationToken.None);

        await _validationContext
            .DidNotReceive()
            .GetById<Site>(
                Arg.Any<Guid>(),
                Arg.Any<CancellationToken>());
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
        _validationContext
            .GetById<Site>(
                Arg.Any<Guid>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Site?>(null));

        var response = await _validator.ValidateAsync(new()
        {
            SiteId = Guid.NewGuid(),
            StoryAddress = _siteRegistered.Address + "/story/1234"
        }, _currentUserContext, CancellationToken.None);

        Assert.IsFalse(response.Success);
        Assert.IsTrue(response.Errors.Any(_ =>
            _.Contains(ValidationMessages.FAILED_TO_FIND_ENTITY) &&
            _.Contains(nameof(Site))));

        await _validationContext
            .Received(1)
            .GetById<Site>(
                Arg.Any<Guid>(),
                Arg.Any<CancellationToken>());
    }

    [TestMethod]
    public async Task ValidateAsync_WhereSiteIdGivenDoesNotMatchStoryAddress_ReturnsError()
    {
        _validationContext
            .GetById<Site>(
                Arg.Any<Guid>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Site?>(_siteRegistered));

        var response = await _validator.ValidateAsync(new()
        {
            SiteId = _siteRegistered.Id,
            StoryAddress = "https://someothersite.net/story/1234"
        }, _currentUserContext, CancellationToken.None);

        Assert.IsFalse(response.Success);
        Assert.IsTrue(response.Errors.Any(_ =>
            _.Contains(ValidationMessages.SITE_STORY_MISMATCH)));

        await _validationContext
            .Received(1)
            .GetById<Site>(
                Arg.Any<Guid>(),
                Arg.Any<CancellationToken>());
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
        _validationContext
            .GetByProperty<Story, string>(
                Arg.Any<Expression<Func<Story, bool>>>(),
                Arg.Is<string>(nameof(Story.ExternalId)),
                Arg.Any<CancellationToken>())
            .Returns(_ => Task.FromResult<Story?>(new Story()));

        _storyInfoProcessor
            .ExtractExternalStoryId(
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
            StoryAddress = _siteRegistered.Address + "/story/1234"
        };
        var response = await _validator.ValidateAsync(request, _currentUserContext, CancellationToken.None);

        Assert.IsTrue(response.Success);
        Assert.AreEqual(_siteRegistered.Id, request.SiteId);
    }
}
