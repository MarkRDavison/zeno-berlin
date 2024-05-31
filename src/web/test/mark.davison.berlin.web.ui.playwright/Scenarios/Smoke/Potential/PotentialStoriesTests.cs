namespace mark.davison.berlin.web.ui.playwright.Scenarios.Smoke.Potential;

[TestClass]
public sealed class PotentialStoriesTests : BerlinBaseTest
{
    [TestMethod]
    public async Task CanAddPotentialStory()
    {
        await Dashboard
            .GoToPage<PotentialStoriesPage>()
            .ThenAsync(_ => _.OpenAddPotentialStoryModal())
            .ThenAsync(_ => _.Submit(StoryUrlHelper.RandomStoryUrl));
    }

    [TestMethod]
    public async Task CanGrabPotentialStory()
    {
        await Dashboard
            .GoToPage<PotentialStoriesPage>()
            .ThenAsync(_ => _.OpenAddPotentialStoryModal())
            .ThenAsync(_ => _.Submit(StoryUrlHelper.RandomStoryUrl))
            .ThenAsync(_ => _.GrabAsync());
    }
}
