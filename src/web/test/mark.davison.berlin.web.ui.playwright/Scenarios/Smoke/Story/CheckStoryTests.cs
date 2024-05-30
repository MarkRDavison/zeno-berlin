namespace mark.davison.berlin.web.ui.playwright.Scenarios.Smoke.CheckStory;

[TestClass]
public sealed class CheckStoryTests : BerlinBaseTest
{
    [TestMethod]
    public async Task CheckingStoryWillTriggerAnUpdate()
    {
        var manageStoryPage = await Dashboard
            .AddStory()
            .ThenAsync(_ => _.AddAsync(StoryUrlHelper.NeverFinishedStoryUrl));

        var (currentChapters, totalChapters) = await manageStoryPage.GetChapters();

        for (var i = 0; i < 3; ++i)
        {
            await manageStoryPage.CheckStoryForUpdates();

            var (updatedCurrentChapters, updatedTotalChapters) = await manageStoryPage.GetChapters();

            updatedTotalChapters.Should().Be(totalChapters);
            updatedCurrentChapters.Should().Be(currentChapters + 1);

            currentChapters = updatedCurrentChapters;
            totalChapters = updatedTotalChapters;
        }
    }
}
