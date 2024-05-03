namespace mark.davison.berlin.shared.commands.Scenarios.UpdateStories;

public class UpdateStoriesCommandProcessor : ICommandProcessor<UpdateStoriesRequest, UpdateStoriesResponse>
{
    private readonly ILogger<UpdateStoriesCommandProcessor> _logger;
    private readonly IRepository _repository;
    private readonly IDateService _dateService;
    private readonly INotificationHub _notificationHub;
    private readonly IFandomService _fandomService;
    private readonly IAuthorService _authorService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IEnumerable<INotificationService> _notificationServices; // TODO: TEMP: REMOVE

    public UpdateStoriesCommandProcessor(
        ILogger<UpdateStoriesCommandProcessor> logger,
        IRepository repository,
        IDateService dateService,
        INotificationHub notificationHub,
        IFandomService fandomService,
        IAuthorService authorService,
        IServiceProvider serviceProvider,
        IEnumerable<INotificationService> notificationServices)
    {
        _logger = logger;
        _repository = repository;
        _dateService = dateService;
        _notificationHub = notificationHub;
        _fandomService = fandomService;
        _authorService = authorService;
        _serviceProvider = serviceProvider;
        _notificationServices = notificationServices;
    }

    public async Task<UpdateStoriesResponse> ProcessAsync(UpdateStoriesRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        await using (_repository.BeginTransaction())
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

                var site = await _repository.GetEntityAsync<Site>(g.Key, cancellationToken); // TODO: Cache
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
                await _repository.UpsertEntitiesAsync(toUpdate);
            }

            if (updates.Any())
            {
                await _repository.UpsertEntitiesAsync(updates);
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
                        IsFavourite = _.Favourite,
                        Fandoms = [.. _.StoryFandomLinks.Select(_ => _.FandomId)],
                        Authors = [.. _.StoryAuthorLinks.Select(_ => _.AuthorId)],
                        LastAuthored = _.LastAuthored
                    }
                )]
            };
        }
    }

    private async Task<List<StoryUpdate>> ProcessStory(Site site, Story story, ICurrentUserContext currentUserContext, IStoryInfoProcessor storyInfoProcessor, CancellationToken cancellationToken)
    {
        List<StoryUpdate> updates = [];
        var info = await storyInfoProcessor.ExtractStoryInfo(story.Address, cancellationToken);

        foreach (var fandomExternalName in info.Fandoms)
        {
            if (story.StoryFandomLinks.All(_ => _.Fandom?.ExternalName != fandomExternalName))
            {
                var fandoms = await _fandomService.GetOrCreateFandomsByExternalNames([fandomExternalName], cancellationToken);

                if (fandoms.FirstOrDefault() is Fandom fandom)
                {
                    var link = CreateStoryFandomLink(story.Id, fandom.Id, currentUserContext.CurrentUser.Id);

                    story.StoryFandomLinks.Add(link);

                    await _repository.UpsertEntityAsync(link);
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

                    await _repository.UpsertEntityAsync(link);
                }
            }
        }

        if (story.TotalChapters != info.TotalChapters ||
            story.CurrentChapters != info.CurrentChapters ||
            story.Complete != info.IsCompleted ||
            story.Name != info.Name)
        {
            var update = new StoryUpdate
            {
                Id = Guid.NewGuid(),
                StoryId = story.Id,
                UserId = story.UserId,
                Complete = info.IsCompleted,
                CurrentChapters = info.CurrentChapters,
                TotalChapters = info.TotalChapters,
                LastAuthored = info.Updated,
                LastModified = _dateService.Now
            };

            updates.Add(update);

            var lastUpdates = await _repository.QueryEntities<StoryUpdate>()
                .Where(_ => _.StoryId == story.Id)
                .OrderByDescending(_ => _.CurrentChapters)
                .Take(1)
                .ToListAsync();
            var lastUpdate = lastUpdates.FirstOrDefault(); // TODO: FirstOrDefaultAsync fails with internal EF stuff

            if (lastUpdate != null)
            {
                for (var chapter = lastUpdate.CurrentChapters + 1; chapter < update.CurrentChapters; ++chapter)
                {
                    updates.Add(new StoryUpdate
                    {
                        Id = Guid.NewGuid(),
                        StoryId = story.Id,
                        UserId = story.UserId,
                        Complete = info.IsCompleted,
                        CurrentChapters = chapter,
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
        // TODO: Look at story.UpdateType to determine if the notification should be sent 
        var builder = new StringBuilder();

        builder.AppendLine("==================== STORY UPDATED ====================");
        builder.AppendLine("|");
        builder.AppendLine($"|  Name: {info.Name}{(info.Name == story.Name ? string.Empty : $" - previously {story.Name}")}");
        builder.AppendLine($"|  Chapters: {info.CurrentChapters}/{(info.TotalChapters?.ToString() ?? "?")} - previously {story.CurrentChapters}/{(story.TotalChapters?.ToString() ?? "?")}");
        builder.AppendLine($"|  Status: {(info.IsCompleted ? "Complete" : "In progress")} - previously {(story.Complete ? "Complete" : "In progress")}");
        builder.AppendLine($"|  Site: {site.LongName}({site.ShortName})");
        builder.AppendLine($"|  Url: {story.Address}");
        builder.AppendLine("|");
        builder.AppendLine("=======================================================");

        _logger.LogInformation("Attempting to send notification for chapter {0} for {1}", info.CurrentChapters, info.Name);

        foreach (var notificationService in _notificationServices)
        {
            _logger.LogInformation("Notification service: {0} enabled status: {1}", notificationService.Settings.SECTION, notificationService.Settings.ENABLED);
        }

        var response = await _notificationHub.SendNotification(builder.ToString());

        _logger.LogInformation("Notification response: {0}", System.Text.Json.JsonSerializer.Serialize(response));

        response.Errors.ForEach(_ => _logger.LogError(_));
        response.Warnings.ForEach(_ => _logger.LogWarning(_));
    }

    public async Task<List<Story>> GetStoriesToUpdate(UpdateStoriesRequest request, CancellationToken cancellationToken)
    {
        var includes = new Expression<Func<Story, object>>[] {
            _ => _.StoryFandomLinks,
            _ => _.StoryFandomLinks.Select(_ => _.Fandom),
            _ => _.StoryAuthorLinks,
            _ => _.StoryAuthorLinks.Select(_ => _.Author)
        };

        var refreshOffset = TimeSpan.FromHours(12);// TODO: Configure/options

        if (request.StoryIds.Any())
        {
            var stories = await _repository.GetEntitiesAsync<Story>(
                _ => request.StoryIds.Contains(_.Id),
                includes,
                cancellationToken);
            // TODO: Override max???
            return stories;
        }
        else
        {
            int max = request.Amount <= 0
                ? 2
                : Math.Min(request.Amount, 10);
            var refreshDate = _dateService.Now.Subtract(refreshOffset);

            var stories = await _repository.QueryEntities<Story>()
                .Include(_ => _.StoryFandomLinks)
                .ThenInclude(_ => _.Fandom)
                .Include(_ => _.StoryAuthorLinks)
                .ThenInclude(_ => _.Author)
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
