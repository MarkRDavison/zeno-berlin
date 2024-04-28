namespace mark.davison.berlin.shared.logic.StoryInfo;

public class Ao3StoryInfoProcessor : IStoryInfoProcessor
{
    private readonly HttpClient _client;
    private readonly IRateLimitService _rateLimitService;

    public Ao3StoryInfoProcessor(
        IHttpClientFactory httpClientFactory,
        IRateLimitServiceFactory rateLimitServiceFactory,
        IOptions<Ao3Config> ao3ConfigOptions
    )
    {
        _client = httpClientFactory.CreateClient(nameof(Ao3StoryInfoProcessor));
        _rateLimitService = rateLimitServiceFactory.CreateRateLimiter(TimeSpan.FromSeconds(ao3ConfigOptions.Value.RATE_DELAY));
    }

    public string ExtractExternalStoryId(string storyAddress)
    {
        if (!storyAddress.StartsWith(SiteConstants.ArchiveOfOurOwn_Address))
        {
            return string.Empty;
        }
        var relative = storyAddress.Replace(SiteConstants.ArchiveOfOurOwn_Address, string.Empty);
        var tokens = relative.Split('/', StringSplitOptions.RemoveEmptyEntries).ToList();

        if (tokens.Count < 2)
        {
            return string.Empty;
        }

        if (tokens[0] != "works")
        {
            return string.Empty;
        }

        return tokens[1];
    }

    public async Task<StoryInfoModel> ExtractStoryInfo(string storyAddress, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(storyAddress + "?view_adult=true")
        };

        await _rateLimitService.Wait(cancellationToken);

        var response = await _client.SendAsync(request);

        var content = await response.Content.ReadAsStringAsync();

        return await ParseStoryInfoFromContent(content);
    }

    private static async Task<StoryInfoModel> ParseStoryInfoFromContent(string content)
    {
        var context = BrowsingContext.New(Configuration.Default);

        var document = await context.OpenAsync(req => req.Content(content));

        var title = document
            .GetElementsByClassName("title heading")?
            .FirstOrDefault()?
            .TextContent?
            .Trim();
        title = HttpUtility.HtmlDecode(title);

        var chapterInfo = document
            .GetElementsByClassName("chapters")?
            .Select(_ => _.TextContent?.Trim())
            .Where(_ => !string.IsNullOrEmpty(_) && _.Contains('/'))?
            .FirstOrDefault()?
            .Split('/', StringSplitOptions.RemoveEmptyEntries);

        if (chapterInfo == null || chapterInfo.Length != 2)
        {
            throw new InvalidDataException();
        }

        int currentChapters;
        if (!int.TryParse(chapterInfo[0], out currentChapters))
        {
            throw new InvalidDataException();
        }

        int? totalChapters;
        if (chapterInfo[1] == "?")
        {
            totalChapters = null;
        }
        else if (int.TryParse(chapterInfo[1], out var tc))
        {
            totalChapters = tc;
        }
        else
        {
            throw new InvalidDataException();
        }

        if (string.IsNullOrEmpty(title))
        {
            throw new InvalidDataException();
        }

        var stats = document.GetElementsByClassName("stats")
            .Where(_ => _.TagName.Equals("dl", StringComparison.InvariantCultureIgnoreCase))
            .ToList();

        var groupedStats = stats
            .SelectMany(_ => _.Children)
            .GroupBy(_ => _.ClassName)
            .Where(_ => !string.IsNullOrEmpty(_.Key))
            .ToDictionary(_ => _.Key!, _ => _.Last().InnerHtml);

        DateOnly published;
        DateOnly updated;

        if (!groupedStats.TryGetValue("published", out var publishedValue) || !DateOnly.TryParse(publishedValue, out published))
        {
            throw new InvalidDataException("Could not extract published date");
        }
        if (!groupedStats.TryGetValue("status", out var updatedValue) || !DateOnly.TryParse(updatedValue, out updated))
        {
            throw new InvalidDataException("Could not extract updated date");
        }

        var fandoms = document.GetElementsByClassName("fandom tags")
            .SelectMany(_ => _.Children)
            .SelectMany(_ => _.GetElementsByTagName("a"))
            .Select(_ => HttpUtility.HtmlDecode(_.InnerHtml))
            .Distinct()
            .ToList();

        var authors = document
            .GetElementsByClassName("byline heading")?
            .SelectMany(_ => _.GetElementsByTagName("a"))
            .Select(_ => HttpUtility.HtmlDecode(_.InnerHtml))
            .Distinct()
            .ToList() ?? [];

        return new StoryInfoModel
        {
            Name = title,
            Authors = authors,
            IsCompleted = currentChapters == totalChapters,
            CurrentChapters = currentChapters,
            TotalChapters = totalChapters,
            Published = published,
            Updated = updated,
            Fandoms = fandoms
        };
    }

    public string GenerateBaseStoryAddress(string storyAddress)
    {
        var storyId = ExtractExternalStoryId(storyAddress);
        if (string.IsNullOrEmpty(storyId))
        {
            return string.Empty;
        }

        return SiteConstants.ArchiveOfOurOwn_Address + "/works/" + storyId;
    }
}
