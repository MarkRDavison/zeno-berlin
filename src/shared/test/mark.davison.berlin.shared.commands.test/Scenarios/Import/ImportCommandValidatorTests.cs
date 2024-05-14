namespace mark.davison.berlin.shared.commands.test.Scenarios.Import;

[TestClass]
public sealed class ImportCommandValidatorTests
{
    private readonly IValidationContext _validationContext;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly ImportCommandValidator _validator;
    private readonly Guid _userId;

    public ImportCommandValidatorTests()
    {
        _userId = Guid.NewGuid();
        _validationContext = Substitute.For<IValidationContext>();
        _currentUserContext = Substitute.For<ICurrentUserContext>();

        _currentUserContext.CurrentUser.Returns(new User { Id = _userId });

        _validator = new(_validationContext);
    }

    [TestMethod]
    public async Task ValidateAsync_LoadsAllStories_ForCurrentUser()
    {
        _validationContext
            .GetAllForUserId<Story>(
                Arg.Is<Guid>(_ => _ == _userId),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new List<Story>()));

        await _validator.ValidateAsync(new(), _currentUserContext, CancellationToken.None);

        await _validationContext
            .Received(1)
            .GetAllForUserId<Story>(
                Arg.Is<Guid>(_ => _ == _userId),
                Arg.Any<CancellationToken>());
    }

    [TestMethod]
    public async Task ValidateAsync_WhereStoriesAlreadyExist_ReturnsWarningAndRemovesDuplicatesFromRequest()
    {
        var existingStories = new List<Story>
        {
            new() { Address = "address1" },
            new() { Address = "address2" },
        };

        _validationContext
            .GetAllForUserId<Story>(
                Arg.Is<Guid>(_ => _ == _userId),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(existingStories));

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
