namespace mark.davison.berlin.web.ui.playwright.Pages;

public sealed class DashboardPage(IPage page, AppSettings appSettings) : FanficBasePage(page, appSettings)
{
    public async Task<AddStoryModal> AddStory()
    {
        return await AddStoryModal.GotoAsync(Page, AppSettings);
    }

}
