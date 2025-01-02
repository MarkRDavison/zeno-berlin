namespace mark.davison.berlin.web.ui.test.Pages;

[TestClass]
public class DashboardTests
{
    private readonly IState<DashboardListState> _dashboardListState;
    private readonly IState<FandomListState> _fandomListState;
    private readonly IState<StartupState> _startupState;
    private readonly IDialogService _dialogService;
    private readonly IStoreHelper _storeHelper;
    private readonly IClientNavigationManager _clientNavigationManager;
    private readonly IDateService _dateService;

    private readonly Dashboard _dashboard;

    public DashboardTests()
    {
        _dashboardListState = Substitute.For<IState<DashboardListState>>();
        _fandomListState = Substitute.For<IState<FandomListState>>();
        _startupState = Substitute.For<IState<StartupState>>();
        _dialogService = Substitute.For<IDialogService>();
        _storeHelper = Substitute.For<IStoreHelper>();
        _clientNavigationManager = Substitute.For<IClientNavigationManager>();
        _dateService = Substitute.For<IDateService>();

        _dashboard = new Dashboard
        {
            DashboardListState = _dashboardListState,
            FandomListState = _fandomListState,
            StartupState = _startupState,
            DialogService = _dialogService,
            StoreHelper = _storeHelper,
            ClientNavigationManager = _clientNavigationManager,
            DateService = _dateService
        };
    }

    [TestMethod]
    public void StoryCardUpdatedText_WorksAsExpected_AcrossYears()
    {
        var tile = new DashboardTileDto
        {
            LastAuthored = new DateOnly(2024, 12, 25)
        };

        _dateService.Today.Returns(new DateOnly(2025, 1, 2));

        var text = _dashboard.StoryCardUpdatedText(tile);

        Assert.IsFalse(text.Contains("one year ago", StringComparison.OrdinalIgnoreCase));
    }
}
