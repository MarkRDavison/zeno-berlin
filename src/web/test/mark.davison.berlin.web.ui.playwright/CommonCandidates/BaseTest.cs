namespace mark.davison.berlin.web.ui.playwright.CommonCandidates;

[TestClass]
public abstract class BaseTest : PlaywrightTest, IAsyncDisposable
{
    private static IBrowser? _browser;
    private static IBrowserContext? _context;
    private readonly Faker _faker;
    protected readonly HttpClient _client;
    private const string _authStateFilename = ".auth.json";

    protected BaseTest()
    {
        _client = new HttpClient();

        AppSettings = CreateAppSettings();

        AuthenticationHelper = new AuthenticationHelper(AppSettings);
        StoryUrlHelper = new StoryUrlHelper(AppSettings);

        _faker = new Faker();
    }

    private static AppSettings CreateAppSettings()
    {
        var appSettings = new AppSettings(); // TODO: BaseClass for settings
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Development.json")
            .Build();

        config.GetRequiredSection(appSettings.Section).Bind(appSettings);

        appSettings.EnsureValid();

        return appSettings;
    }

    private static string AuthStateFullPath(string? tempDir) => $"{tempDir?.TrimEnd('/')}/{_authStateFilename}";

    [AssemblyInitialize]
    public static void AssemblyInitialize(TestContext _)
    {
        var appSettings = CreateAppSettings();

        if (File.Exists(AuthStateFullPath(appSettings.ENVIRONMENT.TEMP_DIR)))
        {
            File.Delete(AuthStateFullPath(appSettings.ENVIRONMENT.TEMP_DIR));
        }
    }

    [AssemblyCleanup]
    public static async Task AssemblyCleanup()
    {
        if (_context != null)
        {
            await _context.DisposeAsync();
        }

        if (_browser != null)
        {
            await _browser.DisposeAsync();
        }
    }

    [TestInitialize]
    public async Task TestInitialize()
    {
        await OnPreTestInitialise();

        _browser ??= await Playwright.Firefox.LaunchAsync(new()
        {
            Headless = !Debug,
            SlowMo = Debug ? 250 : null
        });

        _context ??= await _browser.NewContextAsync(new()
        {

            StorageStatePath = File.Exists(AuthStateFullPath(AppSettings.ENVIRONMENT.TEMP_DIR)) ? AuthStateFullPath(AppSettings.ENVIRONMENT.TEMP_DIR) : null
        });

        CurrentPage = await _context.NewPageAsync();

        await CurrentPage.GotoAsync(AppSettings.ENVIRONMENT.WEB_ORIGIN);

        await OnTestInitialise();
    }

    protected virtual Task OnPreTestInitialise() => Task.CompletedTask;
    protected virtual Task OnTestInitialise() => Task.CompletedTask;

    public async ValueTask DisposeAsync()
    {
        if (TestContext.CurrentTestOutcome == UnitTestOutcome.Failed && !string.IsNullOrEmpty(AppSettings.ENVIRONMENT.TEMP_DIR))
        {
            await CurrentPage.ScreenshotAsync(new PageScreenshotOptions
            {
                Path = AppSettings.ENVIRONMENT.TEMP_DIR + "screenshot_" + TestContext.TestName + Guid.NewGuid().ToString().Replace("-", "_") + ".png",
                Type = ScreenshotType.Png
            });
        }
        else if (TestContext.CurrentTestOutcome == UnitTestOutcome.Passed && !string.IsNullOrEmpty(AppSettings.ENVIRONMENT.TEMP_DIR))
        {
            if (_context != null)
            {
                await _context.StorageStateAsync(new()
                {
                    Path = AuthStateFullPath(AppSettings.ENVIRONMENT.TEMP_DIR)
                });
            }
        }

        await CurrentPage.CloseAsync();

        GC.SuppressFinalize(this);
    }

    protected AppSettings AppSettings { get; }
    protected AuthenticationHelper AuthenticationHelper { get; }
    protected StoryUrlHelper StoryUrlHelper { get; }
    protected IPage CurrentPage { get; set; } = default!;

    protected virtual bool Debug => Debugger.IsAttached;

    protected string GetSentence(int words = 3) => _faker.Lorem.Sentence(words);
    protected string GetNoun() => _faker.Hacker.Noun();

}
