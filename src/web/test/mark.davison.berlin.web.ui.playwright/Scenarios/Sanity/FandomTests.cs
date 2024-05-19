namespace mark.davison.berlin.web.ui.playwright.Scenarios.Sanity;

[TestClass]
public sealed class FandomTests : LoggedInTest
{
    private Task NavigateToFandom() => CanNavigateToFandomsPage();

    [TestMethod]
    public async Task CanNavigateToFandomsPage()
    {
        var fandomsLink = CurrentPage.GetByRole(AriaRole.Link, new PageGetByRoleOptions
        {
            Name = "Fandoms"// TODO: Constants
        });

        await fandomsLink.ClickAsync();

        await Expect(CurrentPage).ToHaveURLAsync(AppSettings.ENVIRONMENT.WEB_ORIGIN + "/fandoms");// TODO: Constants

    }

    [TestMethod]
    public async Task CanAddManualFandom()
    {
        var fandomName = GetSentence();

        await NavigateToFandom();

        await CurrentPage.GetByRole(AriaRole.Button, new PageGetByRoleOptions
        {
            NameString = ButtonNames.Add
        }).ClickAsync();

        await CurrentPage.GetByText("Add Fandom").WaitForAsync();// TODO: Constants/share 

        await CurrentPage.GetByLabel("Name").FillAsync(fandomName);// TODO: Constants/share 

        await CurrentPage.GetByRole(AriaRole.Button, new PageGetByRoleOptions
        {
            NameString = ButtonNames.Save
        }).ClickAsync();

        await Task.Delay(500);

        var newFandomLink = CurrentPage.GetByRole(AriaRole.Link, new PageGetByRoleOptions
        {
            Name = fandomName
        });

        await newFandomLink.WaitForAsync();

        var fandomLinkHref = await newFandomLink.GetAttributeAsync("href");

        Assert.IsNotNull(fandomLinkHref);
        Assert.IsTrue(fandomLinkHref.StartsWith("/fandoms/"));// TODO: Constants/share routes??
    }
}
