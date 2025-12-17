namespace mark.davison.berlin.web.tests.playwright.Scenarios.Sanity;

public sealed class PotentialStoriesTests : BerlinBaseTest
{
    [Test]
    public async Task CanNavigateToPotentialStoriessPage()
    {
        await Dashboard
            .GoToPage<PotentialStoriesPage>();

        await Expect(CurrentPage)
            .ToHaveURLAsync(AppSettings.ENVIRONMENT.WEB_ORIGIN + RoutingHelper.PotentialStoriesRouteStart);

    }
}
