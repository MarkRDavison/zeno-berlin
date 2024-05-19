namespace mark.davison.berlin.web.ui.playwright.PageObjectModels.Modals;

public sealed class EditFandomModal
{
    public const string Name = "Name";
    public const string ParentFandom = "Parent fandom";
    public const string Hidden = "Hidden";
    private readonly IPage _page;

    private EditFandomModal(IPage page)
    {
        _page = page;
    }

    public static async Task<EditFandomModal> GotoAsync(IPage page)
    {
        var pom = new EditFandomModal(page);

        await pom.Open();

        return pom;
    }

    public async Task Open()
    {
        await _page.GetByRole(AriaRole.Button, new PageGetByRoleOptions
        {
            NameString = ButtonNames.Edit
        }).ClickAsync();

        await _page.GetByText(ModalNames.EditFandom).WaitForAsync();
    }

    public async Task SetParent(string parent)
    {
        await ComponentHelpers.SetAutoComplete(_page, ParentFandom, parent);
    }

    public async Task Save()
    {
        await _page.GetByRole(AriaRole.Button, new PageGetByRoleOptions
        {
            NameString = ButtonNames.Save
        }).ClickAsync();

        await Task.Delay(500);
    }

}
