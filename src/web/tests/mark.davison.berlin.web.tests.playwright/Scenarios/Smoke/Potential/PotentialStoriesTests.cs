namespace mark.davison.berlin.web.tests.playwright.Scenarios.Smoke.Potential;

public sealed class PotentialStoriesTests : BerlinBaseTest
{
    [Test]
    public async Task CanAddPotentialStory()
    {
        await Dashboard
            .GoToPage<PotentialStoriesPage>()
            .ThenAsync(_ => _.OpenAddPotentialStoryModal())
            .ThenAsync(_ => _.Submit(StoryUrlHelper.RandomStoryUrl));
    }

    [Test]
    public async Task CanGrabPotentialStory()
    {
        await Dashboard
            .GoToPage<PotentialStoriesPage>()
            .ThenAsync(_ => _.OpenAddPotentialStoryModal())
            .ThenAsync(_ => _.Submit(StoryUrlHelper.RandomStoryUrl))
            .ThenAsync(_ => _.GrabAsync());
    }
}