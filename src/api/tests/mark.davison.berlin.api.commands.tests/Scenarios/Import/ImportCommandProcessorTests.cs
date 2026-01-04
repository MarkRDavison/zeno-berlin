namespace mark.davison.berlin.api.commands.tests.Scenarios.Import;

public sealed class ImportCommandProcessorTests
{
    private readonly Guid _currentUserId = Guid.NewGuid();

    private Mock<ICurrentUserContext> _currentUserContext = default!;
    private Mock<ICommandHandler<AddStoryCommandRequest, AddStoryCommandResponse>> _addStoryCommandHandler = default!;
    private Mock<IDateService> _dateServiceMock;
    private IDbContext<BerlinDbContext> _dbContext = default!;
    private ImportCommandProcessor _processor = default!;

    public ImportCommandProcessorTests()
    {
        _dateServiceMock = new();
        _dateServiceMock.Setup(_ => _.Now).Returns(DateTime.UtcNow);
    }

    [Before(Test)]
    public void BeforeTest()
    {
        _currentUserContext = new();
        _currentUserContext.Setup(_ => _.UserId).Returns(_currentUserId);

        _addStoryCommandHandler = new();

        _dbContext = DbContextHelpers.CreateInMemory<BerlinDbContext>(_ => new(_));
        _processor = new(_dbContext, _addStoryCommandHandler.Object, _dateServiceMock.Object);
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
    public async Task ProcessAsync_WhereAllDataValid_ImportsDataCorrectly()
    {
        var request = new ImportCommandRequest
        {
            Data = new()
            {
                Stories =
                [
                    new() { StoryAddress = "someaddress1", Updates = [ new(), new() ] },
                    new() { StoryAddress = "someaddress2", Updates = [ new(), new(), new(), new() ] },
                    new() { StoryAddress = "someaddress3", Updates = [ new() ] }
                ]
            }
        };

        _addStoryCommandHandler
            .Setup(_ => _.Handle(It.IsAny<AddStoryCommandRequest>(), It.IsAny<ICurrentUserContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AddStoryCommandRequest r, ICurrentUserContext c, CancellationToken ct) =>
            {
                var story = new StoryDto { Address = r.StoryAddress };
                return new AddStoryCommandResponse { Value = story };
            });

        var response = await _processor.ProcessAsync(request, _currentUserContext.Object, CancellationToken.None);

        await Assert.That(response.Success).IsTrue();

        _addStoryCommandHandler.Verify(_ => _.Handle(It.IsAny<AddStoryCommandRequest>(), It.IsAny<ICurrentUserContext>(), It.IsAny<CancellationToken>()), Times.Exactly(request.Data.Stories.Count));

        var expectedStoryUpdates = request.Data.Stories.SelectMany(_ => _.Updates).Count();

        var persistedUpdatesCount = await _dbContext
            .Set<StoryUpdate>()
            .AsNoTracking()
            .CountAsync(CancellationToken.None);

        await Assert.That(persistedUpdatesCount).IsEqualTo(expectedStoryUpdates);
    }

    [Test]
    public async Task ProcessAsync_WhereAllDataValid_WithoutRemoteData_ImportsDataCorrectly()
    {
        var request = new ImportCommandRequest
        {
            AddWithoutRemoteData = true,
            Data = new()
            {
                Stories =
                [
                    new() { StoryAddress = "someaddress1", Updates = [ new(), new() ] },
                    new() { StoryAddress = "someaddress2", Updates = [ new(), new(), new(), new() ] },
                    new() { StoryAddress = "someaddress3", Updates = [ new() ] }
                ]
            }
        };

        _addStoryCommandHandler
            .Setup(_ => _.Handle(It.IsAny<AddStoryCommandRequest>(), It.IsAny<ICurrentUserContext>(), It.IsAny<CancellationToken>()))
            .Returns(async (AddStoryCommandRequest r, ICurrentUserContext c, CancellationToken ct) =>
            {
                await Assert.That(r.AddWithoutRemoteData).IsTrue();
                var story = new StoryDto { Address = r.StoryAddress };
                return new AddStoryCommandResponse { Value = story };
            });

        var response = await _processor.ProcessAsync(request, _currentUserContext.Object, CancellationToken.None);

        await Assert.That(response.Success).IsTrue();

        _addStoryCommandHandler.Verify(_ => _.Handle(It.IsAny<AddStoryCommandRequest>(), It.IsAny<ICurrentUserContext>(), It.IsAny<CancellationToken>()), Times.Exactly(request.Data.Stories.Count));

        var expectedStoryUpdates = request.Data.Stories.SelectMany(_ => _.Updates).Count();

        var persistedUpdatesCount = await _dbContext
            .Set<StoryUpdate>()
            .AsNoTracking()
            .CountAsync(CancellationToken.None);

        await Assert.That(persistedUpdatesCount).IsEqualTo(expectedStoryUpdates);
    }

    [Test]
    public async Task ProcessAsync_WhereStoryFailsToAdd_ReturnsErrors()
    {
        var addStoryError = "addstoryerror";
        var addStoryResponse = new AddStoryCommandResponse { Errors = [addStoryError] };

        var request = new ImportCommandRequest
        {
            Data = new() { Stories = [new() { StoryAddress = "someaddress" }] }
        };

        _addStoryCommandHandler
            .Setup(_ => _.Handle(It.IsAny<AddStoryCommandRequest>(), It.IsAny<ICurrentUserContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(addStoryResponse);

        var response = await _processor.ProcessAsync(request, _currentUserContext.Object, CancellationToken.None);

        await Assert.That(response.Success).IsFalse();
        await Assert.That(response.Errors.Any(_ => _.Contains(ValidationMessages.FAILED_TO_IMPORT) && _.Contains(nameof(Story)) && _.Contains(request.Data.Stories.First().StoryAddress) && _.Contains(addStoryError))).IsTrue();
    }

    [Test]
    public async Task ProcessAsync_WhereStoryRequiresAuthentication_ReturnsWarning()
    {
        var addStoryResponse = new AddStoryCommandResponse
        {
            Value = new StoryDto(),
            Warnings = [ValidationMessages.AUTHENTICATION_REQUIRED]
        };

        var request = new ImportCommandRequest
        {
            Data = new() { Stories = [new() { StoryAddress = "someaddress" }] }
        };

        _addStoryCommandHandler
            .Setup(_ => _.Handle(It.IsAny<AddStoryCommandRequest>(), It.IsAny<ICurrentUserContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(addStoryResponse);

        var response = await _processor.ProcessAsync(request, _currentUserContext.Object, CancellationToken.None);

        await Assert.That(response.Success).IsTrue();
        await Assert.That(response.Warnings).Contains(ValidationMessages.AUTHENTICATION_REQUIRED);
    }

    [Test]
    public async Task ProcessAsync_WhereNoStoriesProvided_ReturnsWarning()
    {
        var request = new ImportCommandRequest { Data = new() };

        var response = await _processor.ProcessAsync(request, _currentUserContext.Object, CancellationToken.None);

        await Assert.That(response.Success).IsTrue();
        await Assert.That(response.Warnings.Any(_ => _.Contains(ValidationMessages.NO_ITEMS) && _.Contains(nameof(Story)))).IsTrue();
    }
}