namespace mark.davison.berlin.web.ui.playwright.Scenarios.Sanity;

[TestClass]
public sealed class StoryTests : BerlinBaseTest
{
    [TestMethod]
    public async Task CanNavigateToStorysPage()
    {
        await Dashboard
            .GoToPage<StoriesPage>();

        await Expect(CurrentPage).ToHaveURLAsync(AppSettings.ENVIRONMENT.WEB_ORIGIN + "/stories");
    }
}
