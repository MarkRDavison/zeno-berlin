namespace mark.davison.berlin.shared.logic.tests.StoryInfo;

public sealed class Ao3StoryInfoProcessorTests
{
    private readonly Ao3StoryInfoProcessor _processor;
    private readonly Mock<ILogger<Ao3StoryInfoProcessor>> _logger;
    private readonly TestHttpMessageHandler _handler;
    private readonly Ao3Config _config;

    public Ao3StoryInfoProcessorTests()
    {
        _handler = new();
        _logger = new();

        _config = new() { RATE_DELAY = 0 };

        _processor = new(new HttpClient(_handler), Options.Create(_config), _logger.Object);
    }

    [Arguments("https://archiveofourown.org/works/47216291/chapters/118921742", "47216291")]
    [Arguments("https://archiveofourown.org/works/47216291", "47216291")]
    [Arguments("https://archiveofourown.org/works", "")]
    [Arguments("https://archiveofourown.org/123/47216291", "")]
    [Test]
    public async Task ExtractExternalStoryId_ReturnsExpectedIds(string address, string externalId)
    {
        var extractedStoryId = _processor.ExtractExternalStoryId(address, SiteConstants.ArchiveOfOurOwn_Address);

        await Assert.That(extractedStoryId).IsEqualTo(externalId);
    }

    [Arguments("https://archiveofourown.org/works/47216291/chapters/118921742", "https://archiveofourown.org/works/47216291")]
    [Arguments("https://archiveofourown.org/works/47216291", "https://archiveofourown.org/works/47216291")]
    [Arguments("https://archiveofourown.org/works", "")]
    [Arguments("https://archiveofourown.org/123/47216291", "")]
    [Test]
    public async Task GenerateBaseStoryAddress_ReturnsExpectedAddress(string address, string baseAddress)
    {
        var extractedBaseAddress = _processor.GenerateBaseStoryAddress(address, SiteConstants.ArchiveOfOurOwn_Address);

        await Assert.That(extractedBaseAddress).IsEqualTo(baseAddress);
    }

    [Test]
    public async Task ExtractStoryInfo_ForExample1_Works()
    {
        _handler.Callback = async (HttpRequestMessage request) =>
        {
            return new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent(await File.ReadAllTextAsync("TestData/ExampleAo3WorkResponse_1.html"))
            };
        };

        var storyUrl = "https://archiveofourown.org/works/123/chapters/47216291";
        var baseStoryUrl = _processor.GenerateBaseStoryAddress(storyUrl, SiteConstants.ArchiveOfOurOwn_Address);

        var storyInfo = await _processor.ExtractStoryInfo(storyUrl, SiteConstants.ArchiveOfOurOwn_Address, CancellationToken.None);

        await Assert.That(storyInfo).IsNotNull();
        await Assert.That(storyInfo!.Name).IsEqualTo("All She Never Wanted");
        await Assert.That(storyInfo.Summary).Contains("As Minister for Magic, Kingsley Shacklebolt knew he was capable of a great many things.");
        await Assert.That(storyInfo.Authors.Count).IsEqualTo(2);
        await Assert.That(storyInfo.Authors).Contains("Ragana62");
        await Assert.That(storyInfo.Authors).Contains("Ragana612");
        await Assert.That(storyInfo.IsCompleted).IsFalse();
        await Assert.That(storyInfo.CurrentChapters).IsEqualTo(57);
        await Assert.That(storyInfo.Published).IsEqualTo(new DateOnly(2023, 5, 16));
        await Assert.That(storyInfo.Updated).IsEqualTo(new DateOnly(2024, 2, 11));
        await Assert.That(storyInfo.TotalChapters).IsNull();
        await Assert.That(storyInfo.Fandoms.Count).IsEqualTo(3);
        await Assert.That(storyInfo.Fandoms).Contains("Harry Potter - J. K. Rowling");
        await Assert.That(storyInfo.Fandoms).Contains("Star Wars Legends: New Jedi Order Series - Various Authors");
        await Assert.That(storyInfo.Fandoms).Contains("Star Wars - All Media Types");
        await Assert.That(storyInfo.ChapterInfo.Count).IsEqualTo(57);
        await Assert.That(storyInfo.ChapterInfo[1].Address).IsEqualTo($"{baseStoryUrl}/chapters/118941742");
        await Assert.That(storyInfo.ChapterInfo[2].Address).IsEqualTo($"{baseStoryUrl}/chapters/118953049");
        await Assert.That(storyInfo.ChapterInfo[10].Address).IsEqualTo($"{baseStoryUrl}/chapters/120110161");
        await Assert.That(storyInfo.ChapterInfo[18].Address).IsEqualTo($"{baseStoryUrl}/chapters/121778461");
    }
}