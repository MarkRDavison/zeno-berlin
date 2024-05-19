namespace mark.davison.berlin.web.ui.playwright.Scenarios.Sanity;

[TestClass]
public sealed class BasicAccessTests : BaseTest
{
    [TestMethod]
    public async Task CanLogIn()
    {
        await AuthenticationHelper.EnsureLoggedIn(CurrentPage);

        await Expect(CurrentPage).ToHaveURLAsync(new Regex(AppSettings.ENVIRONMENT.WEB_ORIGIN));
        await Expect(CurrentPage).ToHaveTitleAsync("Fanfic");// TODO: Constants
    }
}