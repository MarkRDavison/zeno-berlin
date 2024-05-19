namespace mark.davison.berlin.web.ui.playwright.Scenarios.Sanity;

[TestClass]
public sealed class StoryTests : LoggedInTest
{
    [TestMethod]
    public async Task CanNavigateToStorysPage()
    {
        var storysLink = CurrentPage.GetByRole(AriaRole.Link, new PageGetByRoleOptions
        {
            Name = "Stories" // TODO: Constants
        });

        await storysLink.ClickAsync();

        await ExpectToBeOnStoriesPage();
    }
}
