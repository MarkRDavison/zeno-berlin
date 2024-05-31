namespace mark.davison.berlin.web.ui.playwright.Pages;

public sealed class PotentialStoriesPage(IPage page, AppSettings appSettings) : FanficBasePage(page, appSettings)
{
    public async Task<AddPotentialStoryModal> OpenAddPotentialStoryModal()
    {
        return await AddPotentialStoryModal.GotoAsync(Page, AppSettings);
    }
}
