namespace mark.davison.berlin.web.tests.playwright.Scenarios.Sanity;

public class LoggingInTests : BaseTest
{
    [Test]
    public async Task CanNavigateToRootAfterLoggingIn()
    {
        await AuthenticationHelper.EnsureLoggedIn(CurrentPage);
    }
}
