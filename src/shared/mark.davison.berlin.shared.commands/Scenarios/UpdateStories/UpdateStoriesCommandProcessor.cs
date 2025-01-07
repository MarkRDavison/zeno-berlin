namespace mark.davison.berlin.shared.commands.Scenarios.UpdateStories;

public sealed class UpdateStoriesCommandProcessor : ICommandProcessor<UpdateStoriesRequest, UpdateStoriesResponse>
{
    private readonly ILogger<UpdateStoriesCommandProcessor> _logger;
    private readonly IDbContext<BerlinDbContext> _dbContext;
    private readonly IDateService _dateService;
    private readonly INotificationHub _notificationHub;
    private readonly IFandomService _fandomService;
    private readonly IAuthorService _authorService;
    private readonly INotificationCreationService _notificationCreationService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IOptions<Ao3Config> _ao3Config;

    public UpdateStoriesCommandProcessor(
        ILogger<UpdateStoriesCommandProcessor> logger,
        IDbContext<BerlinDbContext> dbContext,
        IDateService dateService,
        INotificationHub notificationHub,
        IFandomService fandomService,
        IAuthorService authorService,
        INotificationCreationService notificationCreationService,
        IOptions<Ao3Config> ao3Config,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _dbContext = dbContext;
        _dateService = dateService;
        _notificationHub = notificationHub;
        _fandomService = fandomService;
        _authorService = authorService;
        _notificationCreationService = notificationCreationService;
        _ao3Config = ao3Config;
        _serviceProvider = serviceProvider;
    }

    public async Task<UpdateStoriesResponse> ProcessAsync(UpdateStoriesRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var toUpdate = await GetStoriesToUpdate(request, cancellationToken);
        var updates = new List<StoryUpdate>();

        if (!toUpdate.Any())
        {
            return new() { Warnings = [ValidationMessages.NO_ITEMS] };
        }

        foreach (var g in toUpdate.GroupBy(_ => _.SiteId))
        {
            if (cancellationToken.IsCancellationRequested) { break; }

            var site = await _dbContext.Set<Site>().FindAsync(g.Key, cancellationToken); // TODO: Cache
            if (site == null) { continue; }

            var storyInfoProcessor = _serviceProvider.GetKeyedService<IStoryInfoProcessor>(site.ShortName);
            if (storyInfoProcessor == null) { continue; }

            foreach (var story in g)
            {
                if (cancellationToken.IsCancellationRequested) { break; }

                var storyUpdates = await ProcessStory(site, story, currentUserContext, storyInfoProcessor, cancellationToken);
                updates.AddRange(storyUpdates);
            }
        }

        if (toUpdate.Any())
        {
            _dbContext.Set<Story>().UpdateRange(toUpdate); // TODO: Replace with upsert???
        }

        if (updates.Any())
        {
            await _dbContext.Set<StoryUpdate>().AddRangeAsync(updates, cancellationToken);
        }

        if (toUpdate.Any() || updates.Any())
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        return new()
        {
            Value = [.. toUpdate.Select(_ =>
                new StoryRowDto
                {
                    StoryId = _.Id,
                    Name = _.Name,
                    CurrentChapters = _.CurrentChapters,
                    TotalChapters = _.TotalChapters,
                    ConsumedChapters = _.ConsumedChapters,
                    IsComplete = _.Complete,
                    UpdateTypeId = _.UpdateTypeId,
                    IsFavourite = _.Favourite,
                    Fandoms = [.. _.StoryFandomLinks.Select(_ => _.FandomId)],
                    Authors = [.. _.StoryAuthorLinks.Select(_ => _.AuthorId)],
                    LastAuthored = _.LastAuthored
                }
            )]
        };
    }

    private async Task<List<StoryUpdate>> ProcessStory(Site site, Story story, ICurrentUserContext currentUserContext, IStoryInfoProcessor storyInfoProcessor, CancellationToken cancellationToken)
    {
        List<StoryUpdate> updates = [];
        var info = await storyInfoProcessor.ExtractStoryInfo(story.Address, site.Address, cancellationToken);

        if (info is null)
        {
            return updates;
        }

        foreach (var fandomExternalName in info.Fandoms)
        {
            if (story.StoryFandomLinks.All(_ => _.Fandom?.ExternalName != fandomExternalName))
            {
                var fandoms = await _fandomService.GetOrCreateFandomsByExternalNames([fandomExternalName], cancellationToken);

                if (fandoms.FirstOrDefault() is Fandom fandom)
                {
                    var link = CreateStoryFandomLink(story.Id, fandom.Id, currentUserContext.CurrentUser.Id);

                    story.StoryFandomLinks.Add(link);

                    await _dbContext.AddAsync(link, cancellationToken);
                }
            }
        }
        foreach (var authorName in info.Authors)
        {
            if (story.StoryAuthorLinks.All(_ => _.Author?.Name != authorName))
            {
                var authors = await _authorService.GetOrCreateAuthorsByName([authorName], site.Id, cancellationToken);

                if (authors.FirstOrDefault() is Author author)
                {
                    var link = CreateStoryAuthorLink(story.Id, author.Id, currentUserContext.CurrentUser.Id);

                    story.StoryAuthorLinks.Add(link);

                    await _dbContext.AddAsync(link, cancellationToken);
                }
            }
        }

        if (story.TotalChapters != info.TotalChapters ||
            story.CurrentChapters != info.CurrentChapters ||
            story.Complete != info.IsCompleted ||
            story.Name != info.Name)
        {
            info.ChapterInfo.TryGetValue(info.CurrentChapters, out var chapterInfo);
            var update = new StoryUpdate
            {
                Id = Guid.NewGuid(),
                StoryId = story.Id,
                UserId = story.UserId,
                Complete = info.IsCompleted,
                CurrentChapters = info.CurrentChapters,
                ChapterAddress = chapterInfo?.Address,
                ChapterTitle = chapterInfo?.Title,
                TotalChapters = info.TotalChapters,
                LastAuthored = info.Updated,
                LastModified = _dateService.Now,
                Created = _dateService.Now
            };

            updates.Add(update);

            var lastUpdate = await _dbContext
                .Set<StoryUpdate>()
                .Where(_ => _.StoryId == story.Id)
                .OrderByDescending(_ => _.CurrentChapters)
                .FirstOrDefaultAsync();

            if (lastUpdate != null)
            {
                for (var chapter = lastUpdate.CurrentChapters + 1; chapter < update.CurrentChapters; ++chapter)
                {
                    info.ChapterInfo.TryGetValue(chapter, out chapterInfo);
                    updates.Add(new StoryUpdate
                    {
                        Id = Guid.NewGuid(),
                        StoryId = story.Id,
                        UserId = story.UserId,
                        Complete = info.IsCompleted,
                        CurrentChapters = chapter,
                        ChapterAddress = chapterInfo?.Address,
                        ChapterTitle = chapterInfo?.Title,
                        TotalChapters = info.TotalChapters,
                        LastAuthored = info.Updated,
                        LastModified = update.LastModified
                    });
                }
            }

            await ProcessNotification(site, story, info, cancellationToken);
        }

        story.TotalChapters = info.TotalChapters;
        story.CurrentChapters = info.CurrentChapters;
        story.Complete = info.IsCompleted;
        story.Name = info.Name;
        story.LastAuthored = info.Updated;
        story.LastModified = _dateService.Now;
        story.LastChecked = _dateService.Now;

        return updates;
    }

    private async Task ProcessNotification(Site site, Story story, StoryInfoModel info, CancellationToken cancellationToken)
    {
        if (story.UpdateTypeId == UpdateTypeConstants.MonthlyWithUpdateId)
        {
            return;
        }
        if (story.UpdateTypeId == UpdateTypeConstants.WhenCompleteId &&
            (story.Complete || !info.IsCompleted))
        {
            return;
        }

        var notificationText = _notificationCreationService.CreateNotification(site, story, info);

        _logger.LogInformation("Attempting to send notification for chapter {0} of {1}", info.CurrentChapters, info.Name);

        var response = await _notificationHub.SendNotification(notificationText);

        _logger.LogInformation("Notification response: {0}", System.Text.Json.JsonSerializer.Serialize(response));

        response.Errors.ForEach(_ => _logger.LogError(_));
        response.Warnings.ForEach(_ => _logger.LogWarning(_));
    }

    public async Task<List<Story>> GetStoriesToUpdate(UpdateStoriesRequest request, CancellationToken cancellationToken)
    {
        var refreshOffset = TimeSpan.FromHours(_ao3Config.Value.NONFAV_UPDATE_DELAY_HOURS);
        var refreshOffsetFav = TimeSpan.FromHours(_ao3Config.Value.FAV_UPDATE_DELAY_HOURS);

        if (request.StoryIds.Any())
        {
            var stories = await _dbContext
                .Set<Story>()
                .Include(_ => _.StoryFandomLinks)
                .ThenInclude(sf => sf.Fandom)
                .Include(_ => _.StoryAuthorLinks)
                .ThenInclude(sa => sa.Author)
                .Where(_ => request.StoryIds.Contains(_.Id))
                .ToListAsync(cancellationToken);

            // TODO: Override max???
            return stories;
        }
        else
        {
            int max = request.Amount <= 0
                ? 2
                : Math.Min(request.Amount, 10);
            var refreshDate = _dateService.Now.Subtract(refreshOffset);
            var refreshDateFav = _dateService.Now.Subtract(refreshOffsetFav);

            // TODO: Order by !fav, where fav OR refresh??? 
            // So either its a non fav and needs checking, or everything else fills up on the favourites
            var stories = await _dbContext
                .Set<Story>()
                .Include(_ => _.StoryFandomLinks)
                .ThenInclude(sf => sf.Fandom)
                .Include(_ => _.StoryAuthorLinks)
                .ThenInclude(sa => sa.Author)
                .Where(_ => !_.Complete && _.LastChecked <= refreshDate)
                .OrderBy(_ => _.LastChecked)
                .Take(max)
                .ToListAsync(cancellationToken);
            return stories;
        }
    }

    // TODO: Duplicate
    private static StoryFandomLink CreateStoryFandomLink(Guid storyId, Guid fandomId, Guid userId)
    {
        return new StoryFandomLink
        {
            Id = Guid.NewGuid(),
            StoryId = storyId,
            FandomId = fandomId,
            UserId = userId
        };
    }
    // TODO: Duplicate
    private static StoryAuthorLink CreateStoryAuthorLink(Guid storyId, Guid authorId, Guid userId)
    {
        return new StoryAuthorLink
        {
            Id = Guid.NewGuid(),
            StoryId = storyId,
            AuthorId = authorId,
            UserId = userId
        };
    }
}
