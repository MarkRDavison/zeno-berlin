namespace mark.davison.berlin.web.tests.playwright.Scenarios.Sanity;

public sealed class StoryTests : BerlinBaseTest
{
    [Test]
    public async Task CanNavigateToStorysPage()
    {
        await Dashboard
            .GoToPage<StoriesPage>();

        await Expect(CurrentPage).ToHaveURLAsync(AppSettings.ENVIRONMENT.WEB_ORIGIN + "/stories");
    }
}
