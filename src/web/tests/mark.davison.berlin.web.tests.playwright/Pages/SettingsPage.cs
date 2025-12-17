namespace mark.davison.berlin.web.tests.playwright.Pages;

public sealed class SettingsPage(IPage page, AppSettings appSettings) : FanficBasePage(page, appSettings)
{
    public async Task<SettingsPage> ExportAsync()
    {
        var button = Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions
        {
            NameString = ButtonNames.Export
        });

        await button.ClickAsync();

        var download = await Page.WaitForDownloadAsync(new PageWaitForDownloadOptions
        {
            Timeout = 30_000.0f
        });

        await Assert.That(download.SuggestedFilename).StartsWith("fanfic_export");

        return this;
    }
}