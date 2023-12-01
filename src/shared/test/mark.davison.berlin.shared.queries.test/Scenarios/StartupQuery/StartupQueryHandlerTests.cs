namespace mark.davison.berlin.shared.queries.test.Scenarios.StartupQuery;

[TestClass]
public class StartupQueryHandlerTests
{
    private readonly Mock<IReadonlyRepository> _repository;
    private readonly Mock<ICurrentUserContext> _currentUserContext;
    private readonly StartupQueryHandler _handler;

    public StartupQueryHandlerTests()
    {
        _repository = new(MockBehavior.Strict);
        _currentUserContext = new(MockBehavior.Strict);

        _handler = new(_repository.Object);

        _repository
            .Setup(_ => _.BeginTransaction())
            .Returns(new StubAsyncDisposable());
    }

    [TestMethod]
    public async Task Handle_RetrievesUserOptions()
    {
        var userOptions = new UserOptions
        {
            IsAdmin = true
        };

        _repository
            .Setup(_ => _.GetEntityAsync(
                It.IsAny<Expression<Func<UserOptions, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(userOptions);

        var request = new StartupQueryRequest { };

        var response = await _handler.Handle(request, _currentUserContext.Object, CancellationToken.None);

        Assert.AreEqual(userOptions.IsAdmin, response.Admin);
    }

    [TestMethod]
    public async Task Handle_WhereOptionsDontExist_ReturnError()
    {
        _repository
            .Setup(_ => _.GetEntityAsync(
                It.IsAny<Expression<Func<UserOptions, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserOptions?)null);

        var request = new StartupQueryRequest { };

        var response = await _handler.Handle(request, _currentUserContext.Object, CancellationToken.None);

        Assert.IsFalse(response.Success);
        Assert.IsTrue(response.Errors.Contains(ValidationMessages.MissingUserOptions));
    }
}
