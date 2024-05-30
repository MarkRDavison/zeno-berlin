namespace mark.davison.berlin.web.ui.playwright.Modals;

public sealed class AddFandomModal : BaseModal
{
    private readonly FandomsPage _fandomsPage;
    private const string SaveFandomButtonName = "Save";
    private const string NameLabel = "Name";

    public AddFandomModal(
        IPage page,
        AppSettings appSettings,
        FandomsPage fandomsPage
    ) : base(
        page,
        appSettings)
    {
        _fandomsPage = fandomsPage;
    }

    public sealed record AddFandomInfo(string Name);

    public async Task<FandomsPage> Submit(AddFandomInfo info)
    {
        await FillField(this, NameLabel, info.Name);

        return await Submit();
    }
    public async Task<FandomsPage> Submit()
    {
        await Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions
        {
            Name = SaveFandomButtonName
        }).ClickAsync();

        return _fandomsPage;
    }
}
