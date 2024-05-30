namespace mark.davison.berlin.web.ui.playwright.Scenarios.Smoke.Manage;

[TestClass]
public sealed class ExportTests : BerlinBaseTest
{
    [TestMethod]
    public async Task CanExportStories()
    {
        var settingsPage = await Dashboard
            .AddStory()
            .ThenAsync(_ => _.AddAsync(StoryUrlHelper.FinishedStoryUrl))
            .ThenAsync(_ => _.GoToSettingsPage());

        await settingsPage.ExportAsync();
    }
}
