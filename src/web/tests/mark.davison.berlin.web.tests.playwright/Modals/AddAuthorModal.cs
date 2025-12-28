namespace mark.davison.berlin.web.tests.playwright.Modals;

public sealed class AddAuthorModal : BaseModal
{
    private readonly AuthorsPage _authorsPage;
    private const string SaveAuthorButtonName = "Save";
    private const string NameLabel = "Name";

    public AddAuthorModal(
        IPage page,
        AppSettings appSettings,
        AuthorsPage authorsPage
    ) : base(
        page,
        appSettings)
    {
        _authorsPage = authorsPage;
    }

    public sealed record AddAuthorInfo(string Name);

    public async Task<AuthorsPage> Submit(AddAuthorInfo info)
    {
        await FillField(this, NameLabel, info.Name);

        return await Submit();
    }

    public async Task<AuthorsPage> Submit()
    {
        await Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions
        {
            Name = SaveAuthorButtonName
        }).ClickAsync();

        return _authorsPage;
    }
}
