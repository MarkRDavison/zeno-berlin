namespace mark.davison.berlin.web.ui.playwright.Helpers;

public abstract class BerlinBaseTest : LoggedInTest
{
    protected DashboardPage Dashboard => new(CurrentPage, AppSettings);

    protected override async Task OnPreTestInitialise()
    {
        Console.Error.WriteLine("""
TODO: Make POM a different library to test assembly?? 
So you can only use the public interface in the actual tests???
""");

        await _client.PostAsync($"{AppSettings.ENVIRONMENT.API_ORIGIN}/api/reset", null);
        await _client.PostAsync($"{AppSettings.ENVIRONMENT.STORY_URL}/api/reset", null);
    }
}
