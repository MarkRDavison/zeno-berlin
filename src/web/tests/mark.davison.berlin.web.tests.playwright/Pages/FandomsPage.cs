using mark.davison.berlin.web.tests.playwright.Modals;

namespace mark.davison.berlin.web.tests.playwright.Pages;

public sealed class FandomsPage(IPage page, AppSettings appSettings) : FanficBasePage(page, appSettings)
{
    public async Task<AddFandomModal> OpenAddFandomModal()
    {
        await Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions
        {
            NameString = ButtonNames.Add
        }).ClickAsync();

        await Page.GetByText(ModalNames.AddFandom).WaitForAsync();

        return new AddFandomModal(Page, AppSettings, this);
    }

    public async Task ValidateTableHasFandoms(params string[] fandomNames)
    {
        foreach (var name in fandomNames)
        {
            await GetFandomLinkInfoByName(name);
        }
    }

    public async Task<LinkInfo> GetFandomLinkInfoByName(string fandomName)
    {
        var newFandomLink = Page.GetByRole(AriaRole.Link, new PageGetByRoleOptions
        {
            Name = fandomName
        });

        await newFandomLink.WaitForAsync();

        var fandomLinkName = await newFandomLink.TextContentAsync();
        var fandomLinkHref = await newFandomLink.GetAttributeAsync("href");

        await Assert.That(fandomLinkName).IsEqualTo(fandomName);
        await Assert.That(fandomLinkHref).IsNotNullOrEmpty().And.StartsWith(RoutingHelper.FandomsRouteStart);

        return new LinkInfo(fandomName, fandomLinkHref!);
    }

    public async Task<ManageFandomPage> NavigateToFandomByName(string fandomName)
    {
        var newFandomLink = Page.GetByRole(AriaRole.Link, new PageGetByRoleOptions
        {
            Name = fandomName
        });

        await newFandomLink.First.ClickAsync();

        return await ManageFandomPage.GotoAsync(Page, AppSettings);
    }
}