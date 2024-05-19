namespace mark.davison.berlin.web.ui.playwright.Scenarios.Smoke.Story;

[TestClass]
public sealed class AddStoryTests : LoggedInTest
{
    [TestMethod]
    public async Task AddingStoryWillDisplayExpectedProperties()
    {
        var addStoryModal = await AddStoryModal.GotoAsync(CurrentPage);

        await addStoryModal.AddAsync(StoryUrlHelper.FinishedStoryUrl);

        var manageStory = await ManageStoryPage.GotoAsync(CurrentPage);

        await manageStory.CheckIsFavourite(false);

        var authorInfo = await manageStory.GetAuthorNamesAndIds();

        Assert.AreEqual(2, authorInfo.Count);
        Assert.IsTrue(authorInfo.Select(_ => _.Item1).Contains(FakeStoryConstants.Avalon_Author1));
        Assert.IsTrue(authorInfo.Select(_ => _.Item1).Contains(FakeStoryConstants.Avalon_Author2));

        var fandomInfo = await manageStory.GetFandomNamesAndIds();

        Assert.AreEqual(2, fandomInfo.Count);
        Assert.IsTrue(fandomInfo.Select(_ => _.Item1).Contains(FakeStoryConstants.Avalon_Fandom1));
        Assert.IsTrue(fandomInfo.Select(_ => _.Item1).Contains(FakeStoryConstants.Avalon_Fandom2));
    }
}
