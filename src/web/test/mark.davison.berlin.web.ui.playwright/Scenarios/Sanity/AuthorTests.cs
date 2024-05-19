namespace mark.davison.berlin.web.ui.playwright.Scenarios.Sanity;

[TestClass]
public sealed class AuthorTests : LoggedInTest
{
    [TestMethod]
    public async Task CanNavigateToAuthorsPage()
    {
        var authorsLink = CurrentPage.GetByRole(AriaRole.Link, new PageGetByRoleOptions
        {
            Name = "Authors"// TODO: Constants
        });

        await authorsLink.ClickAsync();

        await Expect(CurrentPage).ToHaveURLAsync(AppSettings.ENVIRONMENT.WEB_ORIGIN + "/authors");// TODO: Constants
    }
}
