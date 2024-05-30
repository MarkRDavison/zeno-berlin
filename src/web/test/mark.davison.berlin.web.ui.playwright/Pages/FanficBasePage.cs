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

        return await base.GoToPage<TPage>();
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
