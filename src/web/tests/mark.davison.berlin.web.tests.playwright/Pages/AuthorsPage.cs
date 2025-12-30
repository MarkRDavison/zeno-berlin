namespace mark.davison.berlin.web.tests.playwright.Pages;

public sealed class AuthorsPage(IPage page, AppSettings appSettings) : FanficBasePage(page, appSettings)
{
    public async Task<AddAuthorModal> OpenAddAuthorModal()
    {
        await Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions
        {
            NameString = ButtonNames.Add
        }).ClickAsync();

        await Page.GetByText(ModalNames.AddAuthor).WaitForAsync();

        return new AddAuthorModal(Page, AppSettings, this);
    }

    public async Task<LinkInfo> GetAuthorLinkInfoByName(string authorName)
    {
        var newAuthorLink = Page.GetByRole(AriaRole.Link, new PageGetByRoleOptions
        {
            Name = authorName
        });

        await newAuthorLink.WaitForAsync();

        var authorLinkName = await newAuthorLink.TextContentAsync();
        var authorLinkHref = await newAuthorLink.GetAttributeAsync("href");

        await Assert.That(authorLinkName).IsEqualTo(authorName);
        await Assert.That(authorLinkHref).IsNotNullOrEmpty().And.StartsWith(RoutingHelper.AuthorsRouteStart);

        return new LinkInfo(authorName, authorLinkHref!);
    }
}
