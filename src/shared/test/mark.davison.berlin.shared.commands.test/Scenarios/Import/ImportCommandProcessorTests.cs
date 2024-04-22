namespace mark.davison.berlin.shared.commands.test.Scenarios.Import;

[TestClass]
public class ImportCommandProcessorTests
{
    private readonly IRepository _repository;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly ICommandHandler<AddStoryCommandRequest, AddStoryCommandResponse> _addStoryCommandHandler;
    private readonly ImportCommandProcessor _processor;
    private readonly User _currentUser;

    public ImportCommandProcessorTests()
    {
        _currentUser = new()
        {
            Id = Guid.NewGuid()
        };
        _repository = Substitute.For<IRepository>();
        _currentUserContext = Substitute.For<ICurrentUserContext>();
        _addStoryCommandHandler = Substitute.For<ICommandHandler<AddStoryCommandRequest, AddStoryCommandResponse>>();
        _currentUserContext.CurrentUser.Returns(_currentUser);

        _processor = new(_repository, _addStoryCommandHandler);
    }

    [TestMethod]
    public async Task ProcessAsync_WhereAllDataValid_ImportsDataCorrectly()
    {
        var request = new ImportCommandRequest
        {
            Data = new()
            {
                Stories =
                [
                    new()
                    {
                        StoryAddress = "someaddress1",
                        Updates=
                        [
                            new(),
                            new()
                        ]
                    },
                    new()
                    {
                        StoryAddress = "someaddress2",
                        Updates=
                        [
                            new(),
                            new(),
                            new(),
                            new()
                        ]
                    },
                    new()
                    {
                        StoryAddress = "someaddress3",
                        Updates=
                        [
                            new()
                        ]
                    }
                ]
            }
        };

        _addStoryCommandHandler
            .Handle(
                Arg.Any<AddStoryCommandRequest>(),
                Arg.Any<ICurrentUserContext>(),
                Arg.Any<CancellationToken>())
            .Returns(_ =>
            {
                var request = _.Arg<AddStoryCommandRequest>();
                var story = new StoryDto { Address = request.StoryAddress };
                return Task.FromResult(new AddStoryCommandResponse { Value = story });
            });

        var response = await _processor.ProcessAsync(request, _currentUserContext, CancellationToken.None);

        Assert.IsTrue(response.Success);

        await _addStoryCommandHandler
            .Received(request.Data.Stories.Count)
            .Handle(
                Arg.Any<AddStoryCommandRequest>(),
                Arg.Any<ICurrentUserContext>(),
                Arg.Any<CancellationToken>());

        var expectedStoryUpdates = request.Data.Stories.SelectMany(_ => _.Updates).Count();

        await _repository
            .Received(1)
            .UpsertEntitiesAsync<StoryUpdate>(
                Arg.Is<List<StoryUpdate>>(_ => _.Count == expectedStoryUpdates),
                Arg.Any<CancellationToken>());
    }

    [TestMethod]
    public async Task ProcessAsync_WhereStoryFailsToAdd_ReturnsErrors()
    {
        var addStoryError = "addstoryerror";
        var addStoryResponse = new AddStoryCommandResponse
        {
            Errors = [addStoryError]
        };
        var request = new ImportCommandRequest
        {
            Data = new()
            {
                Stories =
                [
                    new(){ StoryAddress = "someaddress"}
                ]
            }
        };

        _addStoryCommandHandler
            .Handle(
                Arg.Any<AddStoryCommandRequest>(),
                Arg.Any<ICurrentUserContext>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(addStoryResponse));

        var response = await _processor.ProcessAsync(request, _currentUserContext, CancellationToken.None);

        Assert.IsFalse(response.Success);

        Assert.IsTrue(response.Errors.Any(_ =>
            _.Contains(ValidationMessages.FAILED_TO_IMPORT) &&
            _.Contains(nameof(Story)) &&
            _.Contains(request.Data.Stories.First().StoryAddress) &&
            _.Contains(addStoryError)));
    }

    [TestMethod]
    public async Task ProcessAsync_WhereNoStoriesProvided_ReturnsWarning()
    {
        var request = new ImportCommandRequest
        {
            Data = new()
        };

        var response = await _processor.ProcessAsync(request, _currentUserContext, CancellationToken.None);

        Assert.IsTrue(response.Success);

        Assert.IsTrue(response.Warnings.Any(_ =>
            _.Contains(ValidationMessages.NO_ITEMS) &&
            _.Contains(nameof(Story))));
    }
}
