namespace mark.davison.berlin.web.ui.playwright.PageObjectModels.Pages;

public sealed class ManageFandomPage
{
    private readonly IPage _page;

    private ManageFandomPage(IPage page, Guid fandomId)
    {
        _page = page;
        FandomId = fandomId;
    }

    public Guid FandomId { get; private set; }

    public static async Task<ManageFandomPage> GotoAsync(IPage page)
    {
        await Assertions.Expect(page).ToHaveURLAsync(
            new Regex("/fandoms/"),// TODO: Constants
            new PageAssertionsToHaveURLOptions
            {
                Timeout = 10_000.0f
            });

        var fandomId = Guid.Parse(page.Url.Split("/").Last());

        var pom = new ManageFandomPage(page, fandomId);

        await page.GetByTestId(DataTestIds.FandomTitle).WaitForAsync();

        return pom;
    }

    public async Task ExpectNoParentFandom()
    {
        await ExpectParentFandom("No parent fandom selected");
    }

    public async Task ExpectParentFandom(string name)
    {
        var locator = _page.GetByTestId(DataTestIds.FandomParentText);

        await locator.WaitForAsync();

        var parentText = await locator.TextContentAsync();

        Assert.AreEqual(name, parentText);
    }

    public async Task<EditFandomModal> EditFandomAsync()
    {
        return await EditFandomModal.GotoAsync(_page);
    }
}
