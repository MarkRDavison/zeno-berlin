namespace mark.davison.berlin.web.ui.playwright.Helpers;

[TestClass]
public abstract class BaseTest : PlaywrightTest, IAsyncDisposable
{
    private IBrowser _browser = default!;
    private readonly Faker _faker;
    private readonly HttpClient _client;

    protected BaseTest()
    {
        _client = new HttpClient();
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Development.json")
            .Build();

        AppSettings = new AppSettings();
        config.GetRequiredSection(AppSettings.Section).Bind(AppSettings);
        AppSettings.EnsureValid();

        AuthenticationHelper = new AuthenticationHelper(AppSettings);
        StoryUrlHelper = new StoryUrlHelper(AppSettings);

        _faker = new Faker();
    }

    [TestInitialize]
    public async Task TestInitialize()
    {
        await _client.PostAsync($"{AppSettings.ENVIRONMENT.API_ORIGIN}/api/reset", null);
        await _client.PostAsync($"{AppSettings.ENVIRONMENT.STORY_URL}/api/reset", null);

        _browser = await Playwright.Chromium.LaunchAsync(new()
        {
            Headless = !Debug,
            SlowMo = Debug ? 1000 : null
        });

        CurrentPage = await _browser.NewPageAsync();

        await CurrentPage.GotoAsync(AppSettings.ENVIRONMENT.WEB_ORIGIN);

        await OnTestInitialise();
    }

    protected virtual Task OnTestInitialise() => Task.CompletedTask;

    public async ValueTask DisposeAsync()
    {
        await _browser.DisposeAsync();
    }

    protected AppSettings AppSettings { get; }
    protected AuthenticationHelper AuthenticationHelper { get; }
    protected StoryUrlHelper StoryUrlHelper { get; }
    protected IPage CurrentPage { get; set; } = default!;

    protected virtual bool Debug => Debugger.IsAttached;

    protected string GetSentence(int words = 3) => _faker.Lorem.Sentence(words);
    protected string GetNoun() => _faker.Hacker.Noun();

    protected async Task ExpectToBeOnStoriesPage()
    {
        await Expect(CurrentPage).ToHaveURLAsync(AppSettings.ENVIRONMENT.WEB_ORIGIN + "/stories");
    }
}
