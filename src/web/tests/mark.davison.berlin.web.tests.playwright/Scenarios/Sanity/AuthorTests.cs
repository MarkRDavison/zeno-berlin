namespace mark.davison.berlin.web.tests.playwright.Scenarios.Sanity;

public sealed class AuthorTests : BerlinBaseTest
{
    [Test]
    public async Task CanNavigateToAuthorsPage()
    {
        await Dashboard
            .GoToPage<AuthorsPage>();

        await Expect(CurrentPage)
            .ToHaveURLAsync(AppSettings.ENVIRONMENT.WEB_ORIGIN + RoutingHelper.AuthorsRouteStart);

    }
}
