namespace mark.davison.berlin.bff.common.test.Authentication;

[TestClass]
public class BerlinCustomZenoAuthenticationActionsTests
{
    private readonly IHttpRepository _httpRepository;
    private readonly IDateService _dateService;
    private readonly IOptions<AppSettings> _appSettingsOptions;
    private readonly IZenoAuthenticationSession _authenticationSession;
    private readonly AppSettings _appSettings;
    private readonly BerlinCustomZenoAuthenticationActions _actions;

    private readonly User _user = new();

    public BerlinCustomZenoAuthenticationActionsTests()
    {
        _httpRepository = Substitute.For<IHttpRepository>();
        _dateService = Substitute.For<IDateService>();
        _appSettingsOptions = Substitute.For<IOptions<AppSettings>>();
        _authenticationSession = Substitute.For<IZenoAuthenticationSession>();
        _appSettings = new();
        _appSettingsOptions.Value.ReturnsForAnyArgs(_ => _appSettings);

        _actions = new(_httpRepository, _dateService, _appSettingsOptions);

        _authenticationSession
            .GetString(ZenoAuthenticationConstants.SessionNames.AccessToken)
            .Returns("TOKEN");
        _authenticationSession
            .SetString(ZenoAuthenticationConstants.SessionNames.User, Arg.Any<string>());

        _httpRepository
            .UpsertEntityAsync(
                Arg.Any<User>(),
                Arg.Any<HeaderParameters>(),
                Arg.Any<CancellationToken>())
            .Returns(_ => Task.FromResult(_.Arg<User?>()));

        _httpRepository
            .GetEntityAsync<User>(
                Arg.Any<QueryParameters>(),
                Arg.Any<HeaderParameters>(),
                Arg.Any<CancellationToken>())
            .Returns(_ => Task.FromResult<User?>(_user));

        _dateService.Now.Returns(DateTime.Now);
    }

    [TestMethod]
    public async Task OnUserAuthenticated_WhereUserDoesntExist_CreatesUser()
    {
        _httpRepository
            .GetEntityAsync<User>(
                Arg.Any<QueryParameters>(),
                Arg.Any<HeaderParameters>(),
                Arg.Any<CancellationToken>())
            .Returns(_ => Task.FromResult<User?>(null));

        _httpRepository
            .UpsertEntityAsync(
                Arg.Any<User>(),
                Arg.Any<HeaderParameters>(),
                Arg.Any<CancellationToken>())
            .Returns(_ => Task.FromResult<User?>(_.Arg<User>()));

        await _actions.OnUserAuthenticated(new(), _authenticationSession, CancellationToken.None);

        await _httpRepository
            .Received(1)
            .UpsertEntityAsync(
                Arg.Any<User>(),
                Arg.Any<HeaderParameters>(),
                Arg.Any<CancellationToken>());
    }
}
