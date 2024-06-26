﻿namespace mark.davison.berlin.shared.commands.test.Scenarios.Import;

[TestClass]
public sealed class ImportCommandProcessorTests
{
    private readonly ICurrentUserContext _currentUserContext;
    private readonly ICommandHandler<AddStoryCommandRequest, AddStoryCommandResponse> _addStoryCommandHandler;
    private readonly User _currentUser;

    private IDbContext<BerlinDbContext> _dbContext = default!;
    private ImportCommandProcessor _processor = default!;

    public ImportCommandProcessorTests()
    {
        _currentUser = new()
        {
            Id = Guid.NewGuid()
        };

        _currentUserContext = Substitute.For<ICurrentUserContext>();
        _addStoryCommandHandler = Substitute.For<ICommandHandler<AddStoryCommandRequest, AddStoryCommandResponse>>();
        _currentUserContext.CurrentUser.Returns(_currentUser);
    }

    [TestInitialize]
    public void Initialize()
    {
        _dbContext = DbContextHelpers.CreateInMemory<BerlinDbContext>(_ => new(_));
        _processor = new(_dbContext, _addStoryCommandHandler);
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
                        Updates =
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

        var persistedUpatesCount = await _dbContext
            .Set<StoryUpdate>()
            .AsNoTracking()
            .CountAsync(CancellationToken.None);

        Assert.AreEqual(expectedStoryUpdates, persistedUpatesCount);
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
