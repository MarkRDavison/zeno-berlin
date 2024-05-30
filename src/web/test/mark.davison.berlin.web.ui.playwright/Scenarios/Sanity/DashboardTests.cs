namespace mark.davison.berlin.web.ui.playwright.Scenarios.Sanity;

[TestClass]
public sealed class DashboardTests : BerlinBaseTest
{
    [TestMethod]
    public async Task CanCreateStory()
    {
        var addStoryModal = await Dashboard
            .AddStory();

        var manageStoryPage = await addStoryModal
            .AddAsync(StoryUrlHelper.RandomStoryUrl);

        manageStoryPage.StoryId
            .Should()
            .NotBeEmpty();
    }
}
