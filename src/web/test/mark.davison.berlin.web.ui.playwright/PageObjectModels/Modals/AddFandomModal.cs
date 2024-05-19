namespace mark.davison.berlin.web.ui.playwright.PageObjectModels.Modals;

public sealed class AddFandomModal
{
    public const string Name = "Name";
    public const string ParentFandom = "Parent fandom";
    private readonly IPage _page;

    private AddFandomModal(IPage page)
    {
        _page = page;
    }

    public static async Task<AddFandomModal> GotoAsync(IPage page)
    {
        var pom = new AddFandomModal(page);

        await pom.Open();

        return pom;
    }

    public async Task Open()
    {
        await _page.GetByRole(AriaRole.Button, new PageGetByRoleOptions
        {
            NameString = ButtonNames.Add
        }).ClickAsync();

        await _page.GetByText(ModalNames.AddFandom).WaitForAsync();
    }
    public async Task SetParent(string parent)
    {
        await ComponentHelpers.SetAutoComplete(_page, ParentFandom, parent);
    }

    public async Task AddAsync(string name)
    {
        await AddAsync(name, string.Empty);
    }

    public async Task AddAsync(string name, string parentName)
    {
        await _page.GetByLabel(Name).FillAsync(name);

        if (!string.IsNullOrEmpty(parentName))
        {
            await ComponentHelpers.SetAutoComplete(_page, ParentFandom, parentName);
        }

        await _page.GetByRole(AriaRole.Button, new PageGetByRoleOptions
        {
            NameString = ButtonNames.Save
        }).ClickAsync();

        await Task.Delay(500);
    }
}
