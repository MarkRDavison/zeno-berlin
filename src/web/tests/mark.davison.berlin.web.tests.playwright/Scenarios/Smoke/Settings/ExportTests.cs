namespace mark.davison.berlin.web.tests.playwright.Scenarios.Smoke.Settings;

public sealed class ExportTests : BerlinBaseTest
{
    [Test]
    public async Task CanExportStories()
    {
        var settingsPage = await Dashboard
            .AddStory()
            .ThenAsync(_ => _.AddAsync(StoryUrlHelper.FinishedStoryUrl))
            .ThenAsync(_ => _.GoToPage<SettingsPage>());

        await settingsPage.ExportAsync();
    }
}
