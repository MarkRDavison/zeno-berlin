namespace mark.davison.berlin.web.ui.playwright.Helpers;

public class AuthenticationHelper
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
        // TODO: After upgrading to cookie auth, try not to have to log in unless you really need to 
        await Assertions.Expect(page).ToHaveTitleAsync(new Regex(ExpectedTitle, RegexOptions.Compiled));

        await page.GetByLabel(UsernameLabel).FillAsync(_appSettings.AUTH.USERNAME);
        await page.GetByLabel(Password).FillAsync(_appSettings.AUTH.PASSWORD);

        var button = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions
        {
            NameString = ButtonNames.SignIn
        });

        await button.ClickAsync();

        await Assertions.Expect(page).ToHaveURLAsync(new Regex(_appSettings.ENVIRONMENT.WEB_ORIGIN));
        await Assertions.Expect(page).ToHaveTitleAsync("Fanfic", new PageAssertionsToHaveTitleOptions
        {
            Timeout = 10_000
        });// TODO: Constants
    }
}
