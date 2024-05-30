namespace mark.davison.berlin.web.ui.playwright.Scenarios.Sanity;

[TestClass]
public sealed class FandomTests : BerlinBaseTest
{
    [TestMethod]
    public async Task CanNavigateToFandomsPage()
    {
        await Dashboard
            .GoToPage<FandomsPage>();

        await Expect(CurrentPage)
            .ToHaveURLAsync(AppSettings.ENVIRONMENT.WEB_ORIGIN + "/fandoms");// TODO: Constants

    }
}
