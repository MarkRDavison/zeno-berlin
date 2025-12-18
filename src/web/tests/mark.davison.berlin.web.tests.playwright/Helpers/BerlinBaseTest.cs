namespace mark.davison.berlin.web.tests.playwright.Helpers;

public abstract class BerlinBaseTest : LoggedInTest
{
    protected DashboardPage Dashboard => new(CurrentPage, AppSettings);

    protected override async Task OnPreTestInitialise()
    {
        await _client.PostAsync($"{AppSettings.ENVIRONMENT.API_ORIGIN}/api/reset", null);
        await _client.PostAsync($"{AppSettings.ENVIRONMENT.STORY_URL}/api/reset", null);
    }
}
