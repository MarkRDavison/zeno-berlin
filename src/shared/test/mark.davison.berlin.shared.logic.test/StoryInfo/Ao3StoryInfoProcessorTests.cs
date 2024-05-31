namespace mark.davison.berlin.shared.logic.test.StoryInfo;

[TestClass]
public sealed class Ao3StoryInfoProcessorTests
{
    private readonly Ao3StoryInfoProcessor _processor;
    private readonly IRateLimitService _rateLimitService;
    private readonly IRateLimitServiceFactory _rateLimitServiceFactory;
    private readonly TestHttpMessageHandler _handler;
    private readonly Ao3Config _config;

    public Ao3StoryInfoProcessorTests()
    {
        _handler = new();
        _rateLimitService = Substitute.For<IRateLimitService>();
        _rateLimitServiceFactory = Substitute.For<IRateLimitServiceFactory>();

        _config = new() { RATE_DELAY = 0 };

        _rateLimitService
            .Wait(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(false));

        _rateLimitServiceFactory
            .CreateRateLimiter(Arg.Any<TimeSpan>())
            .Returns(_rateLimitService);

        _processor = new(new HttpClient(_handler), _rateLimitServiceFactory, Options.Create(_config));
    }

    [DataRow("https://archiveofourown.org/works/47216291/chapters/118921742", "47216291")]
    [DataRow("https://archiveofourown.org/works/47216291", "47216291")]
    [DataRow("https://archiveofourown.org/works", "")]
    [DataRow("https://archiveofourown.org/123/47216291", "")]
    [DataTestMethod]
    public void ExtractExternalStoryId_ReturnsExpectedIds(string address, string externalId)
    {
        var extractedStoryId = _processor.ExtractExternalStoryId(address, SiteConstants.ArchiveOfOurOwn_Address);

        Assert.AreEqual(externalId, extractedStoryId);
    }

    [DataRow("https://archiveofourown.org/works/47216291/chapters/118921742", "https://archiveofourown.org/works/47216291")]
    [DataRow("https://archiveofourown.org/works/47216291", "https://archiveofourown.org/works/47216291")]
    [DataRow("https://archiveofourown.org/works", "")]
    [DataRow("https://archiveofourown.org/123/47216291", "")]
    [DataTestMethod]
    public void GenerateBaseStoryAddress_ReturnsExpectedAddress(string address, string baseAddress)
    {
        var extractedBaseAddress = _processor.GenerateBaseStoryAddress(address, SiteConstants.ArchiveOfOurOwn_Address);

        Assert.AreEqual(baseAddress, extractedBaseAddress);
    }

    [TestMethod]
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

        Assert.AreEqual("All She Never Wanted", storyInfo.Name);
        Assert.IsTrue(storyInfo.Summary.Contains("As Minister for Magic, Kingsley Shacklebolt knew he was capable of a great many things."));
        Assert.AreEqual(2, storyInfo.Authors.Count);
        Assert.IsTrue(storyInfo.Authors.Contains("Ragana62"));
        Assert.IsTrue(storyInfo.Authors.Contains("Ragana612"));
        Assert.AreEqual(false, storyInfo.IsCompleted);
        Assert.AreEqual(57, storyInfo.CurrentChapters);
        Assert.AreEqual(new DateOnly(2023, 5, 16), storyInfo.Published);
        Assert.AreEqual(new DateOnly(2024, 2, 11), storyInfo.Updated);
        Assert.IsNull(storyInfo.TotalChapters);
        Assert.AreEqual(3, storyInfo.Fandoms.Count);
        Assert.IsTrue(storyInfo.Fandoms.Contains("Harry Potter - J. K. Rowling"));
        Assert.IsTrue(storyInfo.Fandoms.Contains("Star Wars Legends: New Jedi Order Series - Various Authors"));
        Assert.IsTrue(storyInfo.Fandoms.Contains("Star Wars - All Media Types"));
        Assert.AreEqual(57, storyInfo.ChapterInfo.Count);
        Assert.AreEqual($"{baseStoryUrl}/chapters/118941742", storyInfo.ChapterInfo[1].Address);
        Assert.AreEqual($"{baseStoryUrl}/chapters/118953049", storyInfo.ChapterInfo[2].Address);
        Assert.AreEqual($"{baseStoryUrl}/chapters/120110161", storyInfo.ChapterInfo[10].Address);
        Assert.AreEqual($"{baseStoryUrl}/chapters/121778461", storyInfo.ChapterInfo[18].Address);

        await _rateLimitService
            .Received(1)
            .Wait(Arg.Any<CancellationToken>());
    }
}
