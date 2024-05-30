namespace mark.davison.berlin.web.ui.playwright.Scenarios.Smoke.Story;

[TestClass]
public sealed class AddStoryTests : BerlinBaseTest
{
    [TestMethod]
    public async Task AddingStoryWillDisplayExpectedProperties()
    {
        var manageStoryPage = await Dashboard
            .AddStory()
            .ThenAsync(_ => _.AddAsync(StoryUrlHelper.FinishedStoryUrl));

        await manageStoryPage.CheckIsFavourite(false);

        var authorInfo = await manageStoryPage.GetAuthorNamesAndIds();

        authorInfo.Should().HaveCount(2);
        authorInfo.Select(_ => _.Item1).Should().Contain(FakeStoryConstants.Avalon_Author1);
        authorInfo.Select(_ => _.Item1).Should().Contain(FakeStoryConstants.Avalon_Author2);

        var fandomInfo = await manageStoryPage.GetFandomNamesAndIds();

        fandomInfo.Should().HaveCount(2);
        fandomInfo.Select(_ => _.Item1).Should().Contain(FakeStoryConstants.Avalon_Fandom1);
        fandomInfo.Select(_ => _.Item1).Should().Contain(FakeStoryConstants.Avalon_Fandom2);
    }

}
