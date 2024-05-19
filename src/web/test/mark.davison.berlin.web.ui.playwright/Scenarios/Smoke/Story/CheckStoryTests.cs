namespace mark.davison.berlin.web.ui.playwright.Scenarios.Smoke.CheckStory;

[TestClass]
public sealed class CheckStoryTests : LoggedInTest
{
    [TestMethod]
    public async Task CheckingStoryWillTriggerAnUpdate()
    {
        var addStoryModal = await AddStoryModal.GotoAsync(CurrentPage);

        await addStoryModal.AddAsync(StoryUrlHelper.NeverFinishedStoryUrl);

        var manageStory = await ManageStoryPage.GotoAsync(CurrentPage);

        var (currentChapters, totalChapters) = await manageStory.GetChapters();

        for (var i = 0; i < 3; ++i)
        {
            await manageStory.CheckStoryForUpdates();

            var (updatedCurrentChapters, updatedTotalChapters) = await manageStory.GetChapters();

            Assert.AreEqual(totalChapters, updatedTotalChapters);

            Assert.AreEqual(currentChapters + 1, updatedCurrentChapters);

            currentChapters = updatedCurrentChapters;
            totalChapters = updatedTotalChapters;
        }
    }
}
