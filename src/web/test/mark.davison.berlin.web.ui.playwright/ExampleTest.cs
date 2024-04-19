namespace mark.davison.berlin.web.ui.playwright;

[TestClass]
public class ExampleTest : PlaywrightTest
{
    [TestMethod]
    public async Task HomePageHasPlaywrightInTitleAndGetStartedLinkLinkingtoTheIntrPage()
    {
        // TODO: https://playwright.dev/dotnet/docs/mock#modify-api-responses
        // Maybe can bypass auth with this??????
        bool DebugThisTest = false;
        await using var browser = await Playwright.Chromium.LaunchAsync(new()
        {
            Headless = !DebugThisTest,
            SlowMo = DebugThisTest ? 1000 : null
        });

        var page = await browser.NewPageAsync();

        if (DebugThisTest)
        {
            await page.PauseAsync();
        }

        await page.GotoAsync("https://playwright.dev");

        // Expect a title "to contain" a substring.
        await Expect(page).ToHaveTitleAsync(new Regex("Playwright"));

        // create a locator
        var getStarted = page.GetByRole(AriaRole.Link, new() { Name = "Get started" });

        // Expect an attribute "to be strictly equal" to the value.
        await Expect(getStarted).ToHaveAttributeAsync("href", "/docs/intro");

        // Click the get started link.
        await getStarted.ClickAsync();

        // Expects the URL to contain intro.
        await Expect(page).ToHaveURLAsync(new Regex(".*intro"));
    }
}