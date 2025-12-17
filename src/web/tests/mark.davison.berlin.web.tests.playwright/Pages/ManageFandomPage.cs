namespace mark.davison.berlin.web.tests.playwright.Pages;

public sealed class ManageFandomPage : FanficBasePage
{
    private ManageFandomPage(
        IPage page,
        AppSettings appSettings,
        Guid fandomId
    ) : base(
        page,
        appSettings)
    {
        FandomId = fandomId;
    }

    public Guid FandomId { get; }

    public static async Task<ManageFandomPage> GotoAsync(IPage page, AppSettings appSettings)
    {
        await Assertions.Expect(page)
            .ToHaveURLAsync(
                new Regex(RoutingHelper.FandomsRouteStart),
                new PageAssertionsToHaveURLOptions
                {
                    Timeout = 5_000.0f
                });

        var fandomId = Guid.Parse(page.Url.Split("/").Last());

        var mfp = new ManageFandomPage(page, appSettings, fandomId);

        await page.GetByTestId(DataTestIds.FandomTitle).WaitForAsync();

        return mfp;
    }
    public async Task ExpectNoParentFandom()
    {
        await ExpectParentFandom("No parent fandom selected"); // TODO: Constant
    }

    public async Task ExpectParentFandom(string name)
    {
        var locator = Page.GetByTestId(DataTestIds.FandomParentText);

        await locator.WaitForAsync();

        var parentText = await locator.TextContentAsync();

        await Assert.That(parentText).IsEqualTo(name);
    }

    public async Task ExpectChildFandomContains(params string[] names)
    {
        var childrenContainer = Page.GetByTestId(DataTestIds.FandomChildrenContainer);

        var childFandomNames = await childrenContainer
            .GetByRole(AriaRole.Link)
            .AllTextContentsAsync();

        foreach (var name in names)
        {
            await Assert.That(childFandomNames).Contains(name);
        }
    }

    public async Task<EditFandomModal> OpenEditFandomModal()
    {
        await Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions
        {
            NameString = ButtonNames.Edit
        }).ClickAsync();

        await Page.GetByText(ModalNames.EditFandom).WaitForAsync();

        return new EditFandomModal(Page, AppSettings, this);
    }
}