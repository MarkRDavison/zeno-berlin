namespace mark.davison.berlin.web.tests.playwright.Scenarios.Smoke.Story;

public sealed class AddStoryTests : BerlinBaseTest
{
    [Test]
    public async Task AddingStoryWillDisplayExpectedProperties()
    {
        var manageStoryPage = await Dashboard
            .AddStory()
            .ThenAsync(_ => _.AddAsync(StoryUrlHelper.FinishedStoryUrl));

        await manageStoryPage.CheckIsFavourite(false);

        var authorInfo = await manageStoryPage.GetAuthorNamesAndIds();

        await Assert.That(authorInfo).Count().IsEqualTo(2);
        await Assert.That(authorInfo.Select(_ => _.Item1))
            .Contains(FakeStoryConstants.Avalon_Author1)
            .And
            .Contains(FakeStoryConstants.Avalon_Author2);

        var fandomInfo = await manageStoryPage.GetFandomNamesAndIds();

        await Assert.That(fandomInfo).Count().IsEqualTo(2);
        await Assert.That(fandomInfo.Select(_ => _.Item1))
            .Contains(FakeStoryConstants.Avalon_Fandom1)
            .And
            .Contains(FakeStoryConstants.Avalon_Fandom2);
    }

}