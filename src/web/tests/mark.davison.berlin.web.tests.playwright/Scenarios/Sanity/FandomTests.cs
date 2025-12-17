namespace mark.davison.berlin.web.tests.playwright.Scenarios.Sanity;

public sealed class FandomTests : BerlinBaseTest
{
    [Test]
    public async Task CanNavigateToFandomsPage()
    {
        await Dashboard
            .GoToPage<FandomsPage>();

        await Expect(CurrentPage)
            .ToHaveURLAsync(AppSettings.ENVIRONMENT.WEB_ORIGIN + RoutingHelper.FandomsRouteStart);

    }
}
