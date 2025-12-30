namespace mark.davison.berlin.web.tests.playwright.CommonCandidates;

public abstract class LoggedInTest : BaseTest
{
    protected override async Task OnTestInitialise()
    {
        await AuthenticationHelper.EnsureLoggedIn(CurrentPage);
    }
}
