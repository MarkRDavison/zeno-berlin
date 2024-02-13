namespace mark.davison.berlin.shared.logic.test.StoryInfo;

[TestClass]
public class Ao3StoryInfoProcessorTests
{
    private readonly Ao3StoryInfoProcessor _processor;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly TestHttpMessageHandler _handler;

    public Ao3StoryInfoProcessorTests()
    {
        _handler = new();
        _httpClientFactory = Substitute.For<IHttpClientFactory>();
        _httpClientFactory
            .CreateClient(nameof(Ao3StoryInfoProcessor))
            .Returns(new HttpClient(_handler));

        _processor = new(_httpClientFactory);
    }

    [DataRow("https://archiveofourown.org/works/47216291/chapters/118921742", "47216291")]
    [DataRow("https://archiveofourown.org/works/47216291", "47216291")]
    [DataRow("https://archiveofourown.org/works", "")]
    [DataRow("https://archiveofourown.org/123/47216291", "")]
    [DataTestMethod]
    public void ExtractExternalStoryId_ReturnsExpectedIds(string address, string externalId)
    {
        var extractedStoryId = _processor.ExtractExternalStoryId(address);

        Assert.AreEqual(externalId, extractedStoryId);
    }

    [DataRow("https://archiveofourown.org/works/47216291/chapters/118921742", "https://archiveofourown.org/works/47216291")]
    [DataRow("https://archiveofourown.org/works/47216291", "https://archiveofourown.org/works/47216291")]
    [DataRow("https://archiveofourown.org/works", "")]
    [DataRow("https://archiveofourown.org/123/47216291", "")]
    [DataTestMethod]
    public void GenerateBaseStoryAddress_ReturnsExpectedAddress(string address, string baseAddress)
    {
        var extractedBaseAddress = _processor.GenerateBaseStoryAddress(address);

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

        var storyInfo = await _processor.ExtractStoryInfo(
            "https://archiveofourown.org/123/47216291/idontmatterforthistest",
            CancellationToken.None);

        Assert.AreEqual("All She Never Wanted", storyInfo.Name);
        Assert.AreEqual(false, storyInfo.IsCompleted);
        Assert.AreEqual(57, storyInfo.CurrentChapters);
        Assert.IsNull(storyInfo.TotalChapters);
    }
}
