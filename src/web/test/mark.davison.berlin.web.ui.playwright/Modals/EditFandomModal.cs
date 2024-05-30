namespace mark.davison.berlin.web.ui.playwright.Modals;

public sealed class EditFandomModal : BaseModal
{
    public const string Name = "Name";
    public const string ParentFandom = "Parent fandom";
    public const string Hidden = "Hidden";
    private readonly ManageFandomPage _manageFandomPage;

    public EditFandomModal(
        IPage page,
        AppSettings appSettings,
        ManageFandomPage manageFandomPage
    ) : base(
        page,
        appSettings)
    {
        _manageFandomPage = manageFandomPage;
    }

    public async Task<EditFandomModal> SetParent(string parent)
    {
        await ComponentHelpers.SetAutoComplete(Page, ParentFandom, parent);

        return this;
    }

    public async Task Submit()
    {
        await Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions
        {
            NameString = ButtonNames.Save
        }).ClickAsync();

        await Assertions.Expect(Page.GetByText(ModalNames.EditFandom)).ToHaveCountAsync(0);
    }
}
