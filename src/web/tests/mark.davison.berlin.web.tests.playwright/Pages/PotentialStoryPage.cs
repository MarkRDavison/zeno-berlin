namespace mark.davison.berlin.web.tests.playwright.Pages;

public sealed class PotentialStoryPage : FanficBasePage
{
    private PotentialStoryPage(
        IPage page,
        AppSettings appSettings,
        Guid potentialStoryId
    ) : base(
        page,
        appSettings)
    {
        PotentialStoryId = potentialStoryId;
    }

    public Guid PotentialStoryId { get; }

    public static async Task<PotentialStoryPage> GotoAsync(IPage page, AppSettings appSettings)
    {
        await Assertions.Expect(page).ToHaveURLAsync(
            new Regex("/potential/"),// TODO: Constants
            new PageAssertionsToHaveURLOptions
            {
                Timeout = 10_000.0f
            });

        var storyId = Guid.Parse(page.Url.Split("/").Last());

        var psp = new PotentialStoryPage(page, appSettings, storyId);

        await page.GetByTestId(DataTestIds.StoryTitle).WaitForAsync();

        return psp;
    }

    public async Task<ManageStoryPage> GrabAsync()
    {
        await Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions
        {
            NameString = ButtonNames.Grab
        }).ClickAsync();

        return await ManageStoryPage.GotoAsync(Page, AppSettings);
    }
}
