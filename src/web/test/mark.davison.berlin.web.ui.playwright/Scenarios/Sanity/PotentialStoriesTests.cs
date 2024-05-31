namespace mark.davison.berlin.web.ui.playwright.Scenarios.Sanity;

[TestClass]
public sealed class PotentialStoriesTests : BerlinBaseTest
{
    [TestMethod]
    public async Task CanNavigateToPotentialStoriesPage()
    {
        await Dashboard
            .GoToPage<PotentialStoriesPage>();

        await Expect(CurrentPage).ToHaveURLAsync(AppSettings.ENVIRONMENT.WEB_ORIGIN + "/potential");
    }
}
