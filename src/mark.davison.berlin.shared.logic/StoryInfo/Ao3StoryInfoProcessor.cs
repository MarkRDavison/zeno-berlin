namespace mark.davison.berlin.shared.logic.StoryInfo;

public class Ao3StoryInfoProcessor : IStoryInfoProcessor
{
    private readonly HttpClient _client;
    private readonly IRateLimitService _rateLimitService;

    public Ao3StoryInfoProcessor(
        IHttpClientFactory httpClientFactory,
        IRateLimitServiceFactory rateLimitServiceFactory
    )
    {
        _client = httpClientFactory.CreateClient(nameof(Ao3StoryInfoProcessor));
        _rateLimitService = rateLimitServiceFactory.CreateRateLimiter(TimeSpan.FromSeconds(3)); // TODO: CONFIG
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

        var context = BrowsingContext.New(Configuration.Default);

        var document = await context.OpenAsync(req => req.Content(content));

        var title = document
            .GetElementsByClassName("title heading")?
            .FirstOrDefault()?
            .TextContent?
            .Trim();

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

        return new StoryInfoModel
        {
            Name = title,
            IsCompleted = currentChapters == totalChapters,
            CurrentChapters = currentChapters,
            TotalChapters = totalChapters
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
