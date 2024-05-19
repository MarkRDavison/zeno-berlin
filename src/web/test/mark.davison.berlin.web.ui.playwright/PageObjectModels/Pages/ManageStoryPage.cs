namespace mark.davison.berlin.web.ui.playwright.PageObjectModels.Pages;

public sealed class ManageStoryPage
{
    private readonly IPage _page;

    private ManageStoryPage(IPage page, Guid storyId)
    {
        _page = page;
        StoryId = storyId;
    }

    public Guid StoryId { get; private set; }

    public static async Task<ManageStoryPage> GotoAsync(IPage page)
    {
        await Assertions.Expect(page).ToHaveURLAsync(
            new Regex("/stories/"),// TODO: Constants
            new PageAssertionsToHaveURLOptions
            {
                Timeout = 10_000.0f
            });

        var storyId = Guid.Parse(page.Url.Split("/").Last());

        var pom = new ManageStoryPage(page, storyId);

        await page.GetByTestId(DataTestIds.StoryTitle).WaitForAsync();

        return pom;
    }

    public async Task CheckStoryForUpdates()
    {
        await _page.GetByRole(AriaRole.Button, new PageGetByRoleOptions
        {
            NameString = ButtonNames.Check
        }).ClickAsync();

        // Wait for the check button to not be disabled
        await _page.GetByRole(AriaRole.Button, new PageGetByRoleOptions
        {
            NameString = ButtonNames.Check
        }).WaitForAsync();
    }

    public async Task CheckIsFavourite(bool isFavourite)
    {
        var icon = _page.GetByTestId(DataTestIds.StoryFavouriteIcon);

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
        await _page.GetByTestId(DataTestIds.StoryFavouriteIcon).ClickAsync();
    }

    public async Task<List<(string, Guid)>> GetAuthorNamesAndIds()
    {
        var authorList = _page.GetByTestId(DataTestIds.StoryAuthorList);

        await _page.ReloadAsync(); // TODO: Temp

        await authorList.WaitForAsync();

        var info = new List<(string, Guid)>();

        var links = await authorList.GetByRole(AriaRole.Link).AllAsync();

        foreach (var authorLink in links)
        {
            var authorName = await authorLink.InnerTextAsync();
            var authorId = await authorLink.GetAttributeAsync("id");

            Assert.IsFalse(string.IsNullOrEmpty(authorName));
            Assert.IsFalse(string.IsNullOrEmpty(authorId));

            Assert.IsTrue(Guid.TryParse(authorId, out var id));
            info.Add((authorName, id));
        }

        return info;
    }

    public async Task<List<(string, Guid)>> GetFandomNamesAndIds()
    {
        var fandomList = _page.GetByTestId(DataTestIds.StoryFandomsList);

        await _page.ReloadAsync(); // TODO: Temp

        await fandomList.WaitForAsync();

        var info = new List<(string, Guid)>();

        var links = await fandomList.GetByRole(AriaRole.Link).AllAsync();

        foreach (var fandomLink in links)
        {
            var fandomName = await fandomLink.InnerTextAsync();
            var fandomId = await fandomLink.GetAttributeAsync("id");

            Assert.IsFalse(string.IsNullOrEmpty(fandomName));
            Assert.IsFalse(string.IsNullOrEmpty(fandomId));

            Assert.IsTrue(Guid.TryParse(fandomId, out var id));
            info.Add((fandomName, id));
        }

        return info;
    }

    public async Task<(int, int?)> GetChapters()
    {
        await Task.Delay(TimeSpan.FromMilliseconds(500));
        var locator = _page.GetByTestId(DataTestIds.StoryChapterText);

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
