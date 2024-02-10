using mark.davison.common.Repository;
using mark.davison.common.server.abstractions.Authentication;
using mark.davison.common.server.abstractions.Identification;

namespace mark.davison.berlin.bff.common.test.Authentication;

[TestClass]
public class BerlinCustomZenoAuthenticationActionsTests
{
    private readonly Mock<IHttpRepository> _httpRepository;
    private readonly Mock<IDateService> _dateService;
    private readonly Mock<IOptions<AppSettings>> _appSettingsOptions;
    private readonly Mock<IZenoAuthenticationSession> _authenticationSession;
    private readonly AppSettings _appSettings;
    private readonly BerlinCustomZenoAuthenticationActions _actions;

    private readonly User _user = new();

    public BerlinCustomZenoAuthenticationActionsTests()
    {
        _httpRepository = new(MockBehavior.Strict);
        _dateService = new(MockBehavior.Strict);
        _appSettingsOptions = new(MockBehavior.Strict);
        _authenticationSession = new(MockBehavior.Strict);
        _appSettings = new()
        {
            ADMIN_USERNAME = "Admin"
        };
        _appSettingsOptions.Setup(_ => _.Value).Returns(() => _appSettings);

        _actions = new(_httpRepository.Object, _dateService.Object, _appSettingsOptions.Object);

        _authenticationSession
            .Setup(_ => _.GetString(ZenoAuthenticationConstants.SessionNames.AccessToken))
            .Returns("TOKEN");

        _authenticationSession
            .Setup(_ => _.SetString(
                ZenoAuthenticationConstants.SessionNames.User,
                It.IsAny<string>()));

        _httpRepository
            .Setup(_ => _.UpsertEntityAsync(
                It.IsAny<User>(),
                It.IsAny<HeaderParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((User u, HeaderParameters h, CancellationToken c) => u);

        _httpRepository
            .Setup(_ => _.GetEntityAsync<User>(
                It.IsAny<QueryParameters>(),
                It.IsAny<HeaderParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => _user);

        _dateService
            .Setup(_ => _.Now)
            .Returns(DateTime.Now);
    }

    [TestMethod]
    public async Task OnUserAuthenticated_WhereUserDoesntExist_CreatesUser()
    {
        _httpRepository
            .Setup(_ => _.GetEntityAsync<User>(
                It.IsAny<QueryParameters>(),
                It.IsAny<HeaderParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => (User?)null);

        _httpRepository
            .Setup(_ => _.UpsertEntityAsync(
                It.IsAny<User>(),
                It.IsAny<HeaderParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((User u, HeaderParameters h, CancellationToken c) =>
            {
                return u;
            })
            .Verifiable();

        await _actions.OnUserAuthenticated(new(), _authenticationSession.Object, CancellationToken.None);

        _httpRepository
            .Verify(
                _ => _.UpsertEntityAsync(
                    It.IsAny<User>(),
                    It.IsAny<HeaderParameters>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }
}
