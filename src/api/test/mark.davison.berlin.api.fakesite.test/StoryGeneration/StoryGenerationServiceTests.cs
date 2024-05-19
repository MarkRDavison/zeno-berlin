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

    [TestMethod]
    public async Task GenerateStoryPage_Works()
    {
        var externalStoryId = FakeStoryConstants.CompleteStoryExternalId;
        var externalChapterId = 133048177;
        var response = await _service.GenerateStoryPage(externalStoryId, externalChapterId, CancellationToken.None);

        var info = await Ao3StoryInfoProcessor.ParseStoryInfoFromContent(
            $"https://idontmatter.com/works/{externalStoryId}",
            response,
            CancellationToken.None);

        Assert.IsFalse(string.IsNullOrEmpty(info.Name));
    }
}
