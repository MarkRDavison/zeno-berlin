namespace mark.davison.berlin.web.ui.playwright.Pages;

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

        download.SuggestedFilename.Should().StartWith("fanfic_export");

        return this;
    }
}
