using HttpMethod = System.Net.Http.HttpMethod;

namespace mark.davison.berlin.shared.logic.StoryInfo;

public sealed class Ao3StoryInfoProcessor : IStoryInfoProcessor
{
    private const string ChapterNumberTitleSeparator = ". ";
    private readonly HttpClient _client;
    private readonly IRateLimitService _rateLimitService;
    private readonly ILogger<Ao3StoryInfoProcessor> _logger;

    public Ao3StoryInfoProcessor(
        HttpClient httpClient,
        IRateLimitServiceFactory rateLimitServiceFactory,
        IOptions<Ao3Config> ao3ConfigOptions,
        ILogger<Ao3StoryInfoProcessor> logger)
    {
        _client = httpClient;
        _rateLimitService = rateLimitServiceFactory.CreateRateLimiter(TimeSpan.FromSeconds(ao3ConfigOptions.Value.RATE_DELAY));
        _logger = logger;
    }

    public string ExtractExternalStoryId(string storyAddress, string siteAddress)
    {
        if (!storyAddress.StartsWith(siteAddress))
        {
            return string.Empty;
        }
        var relative = storyAddress.Replace(siteAddress, string.Empty);
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

    public async Task<StoryInfoModel?> ExtractStoryInfo(string storyAddress, string siteAddress, CancellationToken cancellationToken)
    {
        try
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(storyAddress + "?view_adult=true")
            };

            await _rateLimitService.Wait(cancellationToken);

            var response = await _client.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Extracting AO3 story info for {0} failed - status {1}", request.RequestUri.ToString(), response.StatusCode);

                return null;
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            return await ParseStoryInfoFromContent(GenerateBaseStoryAddress(storyAddress, siteAddress), content, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to extract AO3 story info for {0}", storyAddress + "?view_adult=true");

            return null;
        }
    }

    public static async Task<StoryInfoModel> ParseStoryInfoFromContent(string address, string content, CancellationToken cancellationToken)
    {
        var context = BrowsingContext.New(Configuration.Default);

        var document = await context.OpenAsync(req => req.Content(content), cancellationToken);

        var title = document
            .GetElementsByClassName("title heading")?
            .FirstOrDefault()?
            .TextContent?
            .Trim();
        title = HttpUtility.HtmlDecode(title);

        var summary = document
            .GetElementsByClassName("summary module")?
            .SelectMany(_ => _.GetElementsByTagName("blockquote"))?
            .FirstOrDefault()?
            .InnerHtml?
            .Trim();
        // summary = HttpUtility.HtmlDecode(summary);

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


        if (!groupedStats.TryGetValue("published", out var publishedValue) || !DateOnly.TryParse(publishedValue, out DateOnly published))
        {
            throw new InvalidDataException("Could not extract published date");
        }
        if (!groupedStats.TryGetValue("status", out var updatedValue) || !DateOnly.TryParse(updatedValue, out DateOnly updated))
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

        var chapterValues = document
            .GetElementById("chapter_index")?
            .GetElementsByTagName("option")
            .Select(_ => new { ChapterValue = _.GetAttribute("value"), ChapterTitle = _.InnerHtml })
            .ToList() ?? [];

        Dictionary<int, ChapterInfoModel> chapterInfos = new();

        int chapterNumber = 1;
        foreach (var chapterValueInfo in chapterValues)
        {
            var chapterNumberSeparator = chapterValueInfo.ChapterTitle.IndexOf(ChapterNumberTitleSeparator);

            var chapterTitle = chapterValueInfo.ChapterTitle;

            if (chapterNumberSeparator != -1)
            {
                chapterTitle = chapterTitle.Substring(chapterNumberSeparator + 2);

                if (int.TryParse(chapterValueInfo.ChapterTitle.AsSpan(0, chapterNumberSeparator), out var extractedChapterNumber))
                {
                    if (chapterNumber != extractedChapterNumber)
                    {
                        continue;
                    }
                }

                chapterInfos.Add(chapterNumber, new ChapterInfoModel(chapterNumber, $"{address.TrimEnd('/')}/chapters/{chapterValueInfo.ChapterValue}", chapterTitle));
            }

            chapterNumber++;
        }

        return new StoryInfoModel
        {
            Name = title,
            Summary = summary ?? string.Empty,
            Authors = authors,
            IsCompleted = currentChapters == totalChapters,
            CurrentChapters = currentChapters,
            TotalChapters = totalChapters,
            Published = published,
            ChapterInfo = chapterInfos,
            Updated = updated,
            Fandoms = fandoms
        };
    }

    public string GenerateBaseStoryAddress(string storyAddress, string siteAddress)
    {
        var storyId = ExtractExternalStoryId(storyAddress, siteAddress);
        if (string.IsNullOrEmpty(storyId))
        {
            return string.Empty;
        }

        return siteAddress + "/works/" + storyId;
    }
}
