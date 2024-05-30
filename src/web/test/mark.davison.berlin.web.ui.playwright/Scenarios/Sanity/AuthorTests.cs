namespace mark.davison.berlin.web.ui.playwright.Scenarios.Sanity;

[TestClass]
public sealed class AuthorTests : BerlinBaseTest
{
    [TestMethod]
    public async Task CanNavigateToAuthorsPage()
    {
        await Dashboard
            .GoToPage<AuthorsPage>();

        await Expect(CurrentPage)
            .ToHaveURLAsync(AppSettings.ENVIRONMENT.WEB_ORIGIN + "/authors");
        // TODO: Constants
    }
}
