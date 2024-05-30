
namespace mark.davison.berlin.web.ui.playwright.Modals;

public sealed class AddStoryModal : BaseModal
{
    public const string AddressLabel = "Address";

    public AddStoryModal(IPage page, AppSettings appSettings) : base(page, appSettings)
    {
    }

    public static async Task<AddStoryModal> GotoAsync(IPage page, AppSettings appSettings)
    {
        var pom = new AddStoryModal(page, appSettings);

        await pom.Open();

        return pom;
    }

    public async Task Open()
    {
        // TODO: Ensure on dashboard??? if not there navigate to it

        await Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions
        {
            NameString = ButtonNames.Add
        }).ClickAsync();

        await Page.GetByText(ModalNames.AddStory).WaitForAsync();
    }

    public async Task<ManageStoryPage> AddAsync(string storyUrl)
    {
        await Page.GetByLabel(AddressLabel).FillAsync(storyUrl);

        await Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions
        {
            NameString = ButtonNames.Save
        }).ClickAsync();

        await Assertions
            .Expect(Page.GetByText(ModalNames.AddStory))
            .ToHaveCountAsync(0);

        return await ManageStoryPage.GotoAsync(Page, AppSettings);
    }
}
