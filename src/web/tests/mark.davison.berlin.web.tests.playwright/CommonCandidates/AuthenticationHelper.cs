namespace mark.davison.berlin.web.tests.playwright.CommonCandidates;

public partial class AuthenticationHelper
{
    private readonly AppSettings _appSettings;

    private const string UsernameLabel = "Username or email";
    private const string Password = "Password";
    private const string ExpectedTitle = "Sign in";

    public AuthenticationHelper(AppSettings appSettings)
    {
        _appSettings = appSettings;
    }

    public async Task EnsureLoggedIn(IPage page)
    {
        var username = string.Empty;

        try
        {
            username = await page.GetByTestId(DataTestIds.Username).TextContentAsync(new LocatorTextContentOptions
            {
                Timeout = 10000.0f // TODO: Config
            });
        }
        catch (TimeoutException)
        {

        }

        if (string.IsNullOrEmpty(username))
        {
            var loginLink = page.GetByTestId(DataTestIds.LoginHref);

            await Assertions.Expect(loginLink).ToBeVisibleAsync();

            await loginLink.ClickAsync();

            // TODO: Need a way to specify login flow for each provider
            var loginWithProviderLink = page.GetByText($"Login with {_appSettings.AUTH.PROVIDER}");

            await Assertions.Expect(loginWithProviderLink).ToBeVisibleAsync();

            await loginWithProviderLink.ClickAsync();

            await Assertions.Expect(page).ToHaveTitleAsync(ExpectedTitleRegex(), new PageAssertionsToHaveTitleOptions
            {
                Timeout = 10_000.0f // TODO: Config
            });

            await page.GetByLabel(UsernameLabel).FillAsync(_appSettings.AUTH.USERNAME);
            await page.Locator("#password").FillAsync(_appSettings.AUTH.PASSWORD);

            var button = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions
            {
                NameString = ButtonNames.SignIn
            });

            await button.ClickAsync();
        }

        await Assertions.Expect(page)
            .ToHaveURLAsync(
                new Regex(_appSettings.ENVIRONMENT.WEB_ORIGIN),
                new PageAssertionsToHaveURLOptions
                {
                    Timeout = 10_000.0f// TODO: Config
                });
        await Assertions.Expect(page).ToHaveTitleAsync(_appSettings.APP_TITLE);
    }

    [GeneratedRegex(ExpectedTitle, RegexOptions.Compiled)]
    private static partial Regex ExpectedTitleRegex();
}