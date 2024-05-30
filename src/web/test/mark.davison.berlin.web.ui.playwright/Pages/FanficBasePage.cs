namespace mark.davison.berlin.web.ui.playwright.Pages;

public abstract class FanficBasePage(IPage page, AppSettings appSettings) : BasePage<AppSettings>(page, appSettings)
{
    public override async Task<TPage> GoToPage<TPage>()
    {
        // TODO: ensure nav menu is open

        if (typeof(TPage) == typeof(AuthorsPage))
        {
            var authorsPage = await GoToAuthorsPage();

            return (TPage)(object)authorsPage;
        }
        else if (typeof(TPage) == typeof(FandomsPage))
        {
            var fandomsPage = await GoToFandomsPage();

            return (TPage)(object)fandomsPage;
        }
        else if (typeof(TPage) == typeof(StoriesPage))
        {
            var storiesPage = await GoToStoriesPage();

            return (TPage)(object)storiesPage;
        }
        else if (typeof(TPage) == typeof(SettingsPage))
        {
            var settingsPage = await GoToSettingsPage();

            return (TPage)(object)settingsPage;
        }

        return await base.GoToPage<TPage>();
    }

    public async Task<SettingsPage> GoToSettingsPage()
    {
        await Page
            .GetByTestId(DataTestIds.ManageIcon)
            .ClickAsync();

        await Page
            .GetByText(SettingsMenuName.Settings, new PageGetByTextOptions
            {
                Exact = true
            })
            .ClickAsync();

        return new SettingsPage(Page, AppSettings);
    }


    private async Task<AuthorsPage> GoToAuthorsPage()
    {
        var link = Page.GetByRole(AriaRole.Link,

            new PageGetByRoleOptions
            {
                Name = NavMenuNames.Authors
            });

        await link.ClickAsync();

        return new AuthorsPage(Page, AppSettings);
    }

    private async Task<FandomsPage> GoToFandomsPage()
    {
        var link = Page.GetByRole(AriaRole.Link,

            new PageGetByRoleOptions
            {
                Name = NavMenuNames.Fandoms
            });

        await link.ClickAsync();

        return new FandomsPage(Page, AppSettings);
    }

    private async Task<StoriesPage> GoToStoriesPage()
    {
        var link = Page.GetByRole(AriaRole.Link,

            new PageGetByRoleOptions
            {
                Name = NavMenuNames.Stories
            });

        await link.ClickAsync();

        return new StoriesPage(Page, AppSettings);
    }
}
