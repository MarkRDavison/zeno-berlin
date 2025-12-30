namespace mark.davison.berlin.web.tests.playwright.Scenarios.Smoke.Story;

public sealed class CheckStoryTests : BerlinBaseTest
{
    [Test]
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

            await Assert.That(updatedTotalChapters).IsEqualTo(totalChapters);
            await Assert.That(updatedCurrentChapters).IsEqualTo(currentChapters + 1);

            currentChapters = updatedCurrentChapters;
            totalChapters = updatedTotalChapters;
        }
    }
}