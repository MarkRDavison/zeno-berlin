namespace mark.davison.berlin.api.fakesite.test.StoryGeneration;

[TestClass]
public sealed class StoryGenerationServiceTests
{
    private readonly IStoryGenerationStateService _storyGenerationStateService;
    private readonly StoryGenerationService _service;

    public StoryGenerationServiceTests()
    {
        _storyGenerationStateService = Substitute.For<IStoryGenerationStateService>();

        _service = new(_storyGenerationStateService);
    }

    private StoryGenerationInfo CreatePerpetuallyIncompleteButContinuesStoryInfo(int externalId)
    {
        var chapters = 1;
        return new StoryGenerationInfo
        {
            Title = "The never finished tales of Avalon",
            Summary = "Once upon a time, a long time ago, in a land without time... things continued to keep happening!",
            Notes = "This is the current iteration of the tales of Avalon, it will never end",
            Authors = [FakeStoryConstants.Avalon_Author1, FakeStoryConstants.Avalon_Author2],
            Fandoms = [FakeStoryConstants.Avalon_Fandom1, FakeStoryConstants.Avalon_Fandom2],
            Chapters = Enumerable.Range(1, chapters).Select(_ => $"Chapter {_}").ToList(),
            ChapterIds = Enumerable.Range(0, chapters).Select(_ => Random.Shared.Next(1_000_000, 9_999_999)).ToList(),
            Published = DateOnly.FromDateTime(DateTime.Today).AddDays(-500),
            Updated = DateOnly.FromDateTime(DateTime.Today).AddDays(chapters - 500),
            Bookmarks = 145 + chapters * 50,
            Hits = 27534 + chapters * 50,
            Kudos = 22 + chapters * 5,
            Words = 3_753 * chapters,
            Comments = 1234 + chapters * 10,
            CurrentChapters = chapters,
            TotalChapters = null
        };
    }
    [TestMethod]
    public async Task GenerateStoryPage_Works()
    {
        var externalStoryId = FakeStoryConstants.CompleteStoryExternalId;
        var externalChapterId = 133048177;

        var generationInfo = CreatePerpetuallyIncompleteButContinuesStoryInfo(externalStoryId);

        _storyGenerationStateService
            .RecordGeneration(externalStoryId, externalChapterId)
            .Returns(generationInfo);

        var response = await _service.GenerateStoryPage(externalStoryId, externalChapterId, CancellationToken.None);

        var info = await Ao3StoryInfoProcessor.ParseStoryInfoFromContent(
            $"https://idontmatter.com/works/{externalStoryId}",
            response,
            CancellationToken.None);

        Assert.IsFalse(string.IsNullOrEmpty(info.Name));
    }
}
