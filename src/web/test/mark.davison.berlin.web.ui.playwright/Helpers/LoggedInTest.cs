namespace mark.davison.berlin.web.ui.playwright.Helpers;

public abstract class LoggedInTest : BaseTest
{
    protected override async Task OnTestInitialise()
    {
        await AuthenticationHelper.EnsureLoggedIn(CurrentPage);
    }
}
