namespace mark.davison.berlin.web.ui.playwright.Modals;

public sealed class AddPotentialStoryModal(IPage page, AppSettings appSettings) : BaseModal(page, appSettings)
{
    private const string SavePotentialStoryButtonName = "Save";
    private const string AddressLabel = "Address";


    public static async Task<AddPotentialStoryModal> GotoAsync(IPage page, AppSettings appSettings)
    {
        var pom = new AddPotentialStoryModal(page, appSettings);

        await pom.Open();

        return pom;
    }

    public async Task Open()
    {
        await Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions
        {
            NameString = ButtonNames.Add
        }).ClickAsync();

        await Page.GetByText(ModalNames.AddPotentialStory).WaitForAsync();
    }

    public async Task<PotentialStoryPage> Submit(string address)
    {
        await FillField(this, AddressLabel, address);

        return await Submit();
    }

    public async Task<PotentialStoryPage> Submit()
    {
        await Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions
        {
            Name = SavePotentialStoryButtonName
        }).ClickAsync();

        return await PotentialStoryPage.GotoAsync(Page, AppSettings);
    }
}
