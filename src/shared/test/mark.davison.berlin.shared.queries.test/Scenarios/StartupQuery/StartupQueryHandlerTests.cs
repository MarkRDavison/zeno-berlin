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
}
