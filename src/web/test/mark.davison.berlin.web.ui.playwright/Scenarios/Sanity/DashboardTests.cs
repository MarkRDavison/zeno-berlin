namespace mark.davison.berlin.web.ui.playwright.Scenarios.Sanity;

[TestClass]
public sealed class DashboardTests : LoggedInTest
{
    [TestMethod]
    public async Task CanCreateStory()
    {
        var addStoryModal = await AddStoryModal.GotoAsync(CurrentPage);

        await addStoryModal.AddAsync(StoryUrlHelper.RandomStoryUrl);

        await ManageStoryPage.GotoAsync(CurrentPage);
    }
}
