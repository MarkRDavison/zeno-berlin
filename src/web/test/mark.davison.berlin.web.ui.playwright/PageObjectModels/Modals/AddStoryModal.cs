namespace mark.davison.berlin.web.ui.playwright.PageObjectModels.Modals;

public sealed class AddStoryModal
{
    public const string AddressLabel = "Address";
    private readonly IPage _page;

    private AddStoryModal(IPage page)
    {
        _page = page;
    }

    public static async Task<AddStoryModal> GotoAsync(IPage page)
    {
        var pom = new AddStoryModal(page);

        await pom.Open();

        return pom;
    }

    public async Task Open()
    {
        // TODO: Ensure on dashboard??? if not there navigate to it

        await _page.GetByRole(AriaRole.Button, new PageGetByRoleOptions
        {
            NameString = ButtonNames.Add
        }).ClickAsync();

        await _page.GetByText(ModalNames.AddStory).WaitForAsync();
    }

    public async Task AddAsync(string storyUrl)
    {
        await _page.GetByLabel(AddressLabel).FillAsync(storyUrl);

        await _page.GetByRole(AriaRole.Button, new PageGetByRoleOptions
        {
            NameString = ButtonNames.Save
        }).ClickAsync();

        await Task.Delay(500);
    }
}
