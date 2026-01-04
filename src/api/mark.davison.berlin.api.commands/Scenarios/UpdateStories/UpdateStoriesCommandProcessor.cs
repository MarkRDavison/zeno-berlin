namespace mark.davison.berlin.api.commands.Scenarios.UpdateStories;

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
        var mutatedStories = new List<Story>();

        if (!toUpdate.Any())
        {
            _logger.LogTrace("Did not find any stories to update.");
            return new() { Warnings = [ValidationMessages.NO_ITEMS] };
        }

        _logger.LogTrace("Found {0} stories to update.", toUpdate.Count);

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

                if (storyInfoProcessor.HasSiteFailure)
                {
                    _logger.LogWarning("General site failure detected for {0} - cancelling current update process", site.ShortName);
                    break;
                }

                var storyUpdates = await ProcessStory(site, story, currentUserContext, storyInfoProcessor, cancellationToken);

                if (storyUpdates.SuccessWithValue)
                {
                    updates.AddRange(storyUpdates.Value);
                }
                else
                {
                    if (storyUpdates.Errors.Contains(ValidationMessages.AUTHENTICATION_REQUIRED) &&
                        !story.RequiresAuthentication)
                    {
                        story.RequiresAuthentication = true;
                        mutatedStories.Add(story);
                    }
                }
            }
        }

        if (toUpdate.Any())
        {
            _dbContext.Set<Story>().UpdateRange(toUpdate); // TODO: Replace with upsert???
        }

        if (mutatedStories.Any())
        {
            _dbContext.Set<Story>().UpdateRange(mutatedStories);
        }

        if (updates.Any())
        {
            await _dbContext.Set<StoryUpdate>().AddRangeAsync(updates, cancellationToken);
        }

        if (toUpdate.Any() || updates.Any() || mutatedStories.Any())
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
                    LastAuthored = _.LastAuthored,
                }
            )]
        };
    }

    private async Task<Response<List<StoryUpdate>>> ProcessStory(Site site, Story story, ICurrentUserContext currentUserContext, IStoryInfoProcessor storyInfoProcessor, CancellationToken cancellationToken)
    {
        var info = await storyInfoProcessor.ExtractStoryInfo(story.Address, site.Address, cancellationToken);

        if (!info.SuccessWithValue)
        {
            return new Response<List<StoryUpdate>>
            {
                Errors = [.. info.Errors]
            };
        }

        List<StoryUpdate> updates = [];

        foreach (var fandomExternalName in info.Value.Fandoms)
        {
            if (story.StoryFandomLinks.All(_ => _.Fandom?.ExternalName != fandomExternalName))
            {
                var fandoms = await _fandomService.GetOrCreateFandomsByExternalNames([fandomExternalName], cancellationToken);

                if (fandoms.FirstOrDefault() is Fandom fandom)
                {
                    var link = CreateStoryFandomLink(story.Id, fandom.Id, currentUserContext.UserId);

                    story.StoryFandomLinks.Add(link);

                    await _dbContext.AddAsync(link, cancellationToken);
                }
            }
        }

        foreach (var authorName in info.Value.Authors)
        {
            if (story.StoryAuthorLinks.All(_ => _.Author?.Name != authorName))
            {
                var authors = await _authorService.GetOrCreateAuthorsByName([authorName], site.Id, cancellationToken);

                if (authors.FirstOrDefault() is Author author)
                {
                    var link = CreateStoryAuthorLink(story.Id, author.Id, currentUserContext.UserId);

                    story.StoryAuthorLinks.Add(link);

                    await _dbContext.AddAsync(link, cancellationToken);
                }
            }
        }

        if (story.TotalChapters != info.Value.TotalChapters ||
            story.CurrentChapters != info.Value.CurrentChapters ||
            story.Complete != info.Value.IsCompleted ||
            story.Name != info.Value.Name)
        {
            info.Value.ChapterInfo.TryGetValue(info.Value.CurrentChapters, out var chapterInfo);

            StoryUpdate? update = null;

            var lastUpdate = await _dbContext
                .Set<StoryUpdate>()
                .Where(_ => _.StoryId == story.Id)
                .OrderByDescending(_ => _.CurrentChapters)
                .FirstOrDefaultAsync();

            update = lastUpdate;

            if (lastUpdate is null || lastUpdate.CurrentChapters < info.Value.CurrentChapters)
            {
                update = new StoryUpdate
                {
                    Id = Guid.NewGuid(),
                    StoryId = story.Id,
                    UserId = story.UserId,
                    Complete = info.Value.IsCompleted,
                    CurrentChapters = info.Value.CurrentChapters,
                    ChapterAddress = chapterInfo?.Address,
                    ChapterTitle = chapterInfo?.Title,
                    TotalChapters = info.Value.TotalChapters,
                    LastAuthored = info.Value.Updated,
                    LastModified = _dateService.Now,
                    Created = _dateService.Now
                };

                updates.Add(update);
            }

            if (lastUpdate != null)
            {
                for (var chapter = lastUpdate.CurrentChapters + 1; chapter < update!.CurrentChapters; ++chapter)
                {
                    info.Value.ChapterInfo.TryGetValue(chapter, out chapterInfo);
                    updates.Add(new StoryUpdate
                    {
                        Id = Guid.NewGuid(),
                        StoryId = story.Id,
                        UserId = story.UserId,
                        Complete = info.Value.IsCompleted,
                        CurrentChapters = chapter,
                        ChapterAddress = chapterInfo?.Address,
                        ChapterTitle = chapterInfo?.Title,
                        TotalChapters = info.Value.TotalChapters,
                        LastAuthored = info.Value.Updated,
                        LastModified = update.LastModified,
                        Created = _dateService.Now
                    });
                }
            }

            await ProcessNotification(site, story, info.Value, cancellationToken);
        }

        story.TotalChapters = info.Value.TotalChapters;
        story.CurrentChapters = info.Value.CurrentChapters;
        story.Complete = info.Value.IsCompleted;
        story.Name = info.Value.Name;
        story.LastAuthored = info.Value.Updated;
        story.LastModified = _dateService.Now;
        story.LastChecked = _dateService.Now;

        return new Response<List<StoryUpdate>>
        {
            Value = updates
        };
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

        var response = await _notificationHub.SendNotification(new NotificationMessage { Message = notificationText });

        _logger.LogInformation("Notification response: {0}", System.Text.Json.JsonSerializer.Serialize(response));

        response.Errors.ForEach(_ => _logger.LogError(_));
        response.Warnings.ForEach(_ => _logger.LogWarning(_));
    }

    public async Task<List<Story>> GetStoriesToUpdate(UpdateStoriesRequest request, CancellationToken cancellationToken)
    {
        // TODO: Change from hours to timespan
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
                .Where(_ => !_.RequiresAuthentication && request.StoryIds.Contains(_.Id))
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
                .Where(_ => !_.Complete && !_.RequiresAuthentication && _.LastChecked <= refreshDate)
                .OrderBy(_ => _.LastChecked)
                .Take(max)
                .ToListAsync(cancellationToken);
            return stories;
        }
    }

    // TODO: Duplicate
    private StoryFandomLink CreateStoryFandomLink(Guid storyId, Guid fandomId, Guid userId)
    {
        return new StoryFandomLink
        {
            Id = Guid.NewGuid(),
            StoryId = storyId,
            FandomId = fandomId,
            UserId = userId,
            Created = _dateService.Now,
            LastModified = _dateService.Now
        };
    }
    // TODO: Duplicate
    private StoryAuthorLink CreateStoryAuthorLink(Guid storyId, Guid authorId, Guid userId)
    {
        return new StoryAuthorLink
        {
            Id = Guid.NewGuid(),
            StoryId = storyId,
            AuthorId = authorId,
            UserId = userId,
            Created = _dateService.Now,
            LastModified = _dateService.Now
        };
    }
}
