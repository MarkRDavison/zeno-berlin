namespace mark.davison.berlin.web.tests.playwright.Pages;

public sealed class ManageStoryPage : FanficBasePage
{
    public ManageStoryPage(
        IPage page,
        AppSettings appSettings,
        Guid storyId
    ) : base(
        page,
        appSettings)
    {
        StoryId = storyId;
    }

    public Guid StoryId { get; }

    public static async Task<ManageStoryPage> GotoAsync(IPage page, AppSettings appSettings)
    {
        await Assertions.Expect(page).ToHaveURLAsync(
            new Regex("/stories/"),// TODO: Constants
            new PageAssertionsToHaveURLOptions
            {
                Timeout = 10_000.0f
            });

        var storyId = Guid.Parse(page.Url.Split("/").Last());

        var msp = new ManageStoryPage(page, appSettings, storyId);

        await page.GetByTestId(DataTestIds.StoryTitle).WaitForAsync();

        return msp;
    }

    public async Task CheckStoryForUpdates()
    {
        await Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions
        {
            NameString = ButtonNames.Check
        }).ClickAsync();

        // Wait for the check button to not be disabled
        await Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions
        {
            NameString = ButtonNames.Check
        }).WaitForAsync();
    }

    public async Task CheckIsFavourite(bool isFavourite)
    {
        var icon = Page.GetByTestId(DataTestIds.StoryFavouriteIcon);

        await icon.WaitForAsync();

        if (isFavourite)
        {
            await icon.Locator(".favourite-icon").WaitForAsync();
        }
        else
        {
            await icon.Locator(".non-favourite-icon").WaitForAsync();
        }
    }

    public async Task ToggleFavourite()
    {
        await Page.GetByTestId(DataTestIds.StoryFavouriteIcon).ClickAsync();
    }

    public async Task<List<(string, Guid)>> GetAuthorNamesAndIds()
    {
        var authorList = Page.GetByTestId(DataTestIds.StoryAuthorList);

        await Page.ReloadAsync(); // TODO: Temp

        await authorList.WaitForAsync();

        var info = new List<(string, Guid)>();

        var links = await authorList.GetByRole(AriaRole.Link).AllAsync();

        foreach (var authorLink in links)
        {
            var authorName = await authorLink.InnerTextAsync();
            var authorId = await authorLink.GetAttributeAsync("id");

            await Assert.That(authorName).IsNotNullOrEmpty();
            await Assert.That(authorId).IsNotNullOrEmpty();

            _ = Guid.TryParse(authorId, out var id);

            await Assert.That(id).IsNotEmptyGuid();

            info.Add((authorName, id));
        }

        return info;
    }

    public async Task<List<(string, Guid)>> GetFandomNamesAndIds()
    {
        var fandomList = Page.GetByTestId(DataTestIds.StoryFandomsList);

        await Page.ReloadAsync(); // TODO: Temp

        await fandomList.WaitForAsync();

        var info = new List<(string, Guid)>();

        var links = await fandomList.GetByRole(AriaRole.Link).AllAsync();

        foreach (var fandomLink in links)
        {
            var fandomName = await fandomLink.InnerTextAsync();
            var fandomId = await fandomLink.GetAttributeAsync("id");

            await Assert.That(fandomName).IsNotNullOrEmpty();
            await Assert.That(fandomId).IsNotNullOrEmpty();

            _ = Guid.TryParse(fandomId, out var id);

            await Assert.That(id).IsNotEmptyGuid();

            info.Add((fandomName, id));
        }

        return info;
    }

    public async Task<(int, int?)> GetChapters()
    {
        await Task.Delay(TimeSpan.FromMilliseconds(500));
        var locator = Page.GetByTestId(DataTestIds.StoryChapterText);

        await locator.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible
        });

        var text = await locator.InnerTextAsync();
        var parts = text
            .Split(
                [" ", "/"],
                StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .ToList();

        if (parts.Count == 3)
        {
            if (int.TryParse(parts[1], out var currentChapters))
            {
                if (parts[2] == "?")
                {
                    return (currentChapters, null);
                }
                else if (int.TryParse(parts[2], out var totalChapters))
                {
                    return (currentChapters, totalChapters);
                }
            }
        }

        throw new InvalidOperationException("Could not find chapter text");
    }
}
