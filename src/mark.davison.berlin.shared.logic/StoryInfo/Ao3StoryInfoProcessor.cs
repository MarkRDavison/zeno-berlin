namespace mark.davison.berlin.shared.logic.StoryInfo;

public sealed class Ao3StoryInfoProcessor : IStoryInfoProcessor
{
    private const string ChapterNumberTitleSeparator = ". ";
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

        var response = await _client.SendAsync(request, cancellationToken);

        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        return await ParseStoryInfoFromContent(GenerateBaseStoryAddress(storyAddress), content, cancellationToken);
    }

    private static async Task<StoryInfoModel> ParseStoryInfoFromContent(string address, string content, CancellationToken cancellationToken)
    {
        var context = BrowsingContext.New(Configuration.Default);

        var document = await context.OpenAsync(req => req.Content(content), cancellationToken);

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
