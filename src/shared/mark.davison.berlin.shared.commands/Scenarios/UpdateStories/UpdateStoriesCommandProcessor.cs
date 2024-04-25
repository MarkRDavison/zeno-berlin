﻿using mark.davison.common.server.abstractions.Notifications;
using mark.davison.shared.server.services.Helpers;
using System.Linq.Expressions;

namespace mark.davison.berlin.shared.commands.Scenarios.UpdateStories;

public class UpdateStoriesCommandProcessor : ICommandProcessor<UpdateStoriesRequest, UpdateStoriesResponse>
{
    private readonly ILogger<UpdateStoriesCommandProcessor> _logger;
    private readonly IRepository _repository;
    private readonly IDateService _dateService;
    private readonly INotificationHub _notificationHub;
    private readonly IFandomService _fandomService;
    private readonly IServiceProvider _serviceProvider;

    public UpdateStoriesCommandProcessor(
        ILogger<UpdateStoriesCommandProcessor> logger,
        IRepository repository,
        IDateService dateService,
        INotificationHub notificationHub,
        IFandomService fandomService,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _repository = repository;
        _dateService = dateService;
        _notificationHub = notificationHub;
        _fandomService = fandomService;
        _serviceProvider = serviceProvider;
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

                    var update = await ProcessStory(site, story, currentUserContext, storyInfoProcessor, cancellationToken);
                    if (update != null)
                    {
                        updates.Add(update);
                    }
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

            return new() { Value = [.. toUpdate.Select(_ => _.ToDto())] };
        }
    }

    private async Task<StoryUpdate?> ProcessStory(Site site, Story story, ICurrentUserContext currentUserContext, IStoryInfoProcessor storyInfoProcessor, CancellationToken cancellationToken)
    {
        StoryUpdate? update = null;
        var info = await storyInfoProcessor.ExtractStoryInfo(story.Address, cancellationToken);

        foreach (var fandomExternalName in info.Fandoms)
        {
            if (story.StoryFandomLinks.All(_ => _.Fandom?.ExternalName != fandomExternalName))
            {
                var fandoms = await _fandomService.GetOrCreateFandomsByExternalNames([fandomExternalName], cancellationToken);

                var fandom = fandoms.First();

                var link = CreateStoryFandomLink(story.Id, fandom.Id, currentUserContext.CurrentUser.Id);

                story.StoryFandomLinks.Add(link);
            }
        }

        if (story.TotalChapters != info.TotalChapters ||
            story.CurrentChapters != info.CurrentChapters ||
            story.Complete != info.IsCompleted ||
            story.Name != info.Name)
        {
            update = new StoryUpdate
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

            await ProcessNotification(site, story, info, cancellationToken);
        }

        story.TotalChapters = info.TotalChapters;
        story.CurrentChapters = info.CurrentChapters;
        story.Complete = info.IsCompleted;
        story.Name = info.Name;
        story.LastAuthored = info.Updated;
        story.LastModified = _dateService.Now;
        story.LastChecked = _dateService.Now;

        return update;
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

        var response = await _notificationHub.SendNotification(builder.ToString());

        response.Errors.ForEach(_ => _logger.LogError(_));
        response.Warnings.ForEach(_ => _logger.LogWarning(_));
    }

    public async Task<List<Story>> GetStoriesToUpdate(UpdateStoriesRequest request, CancellationToken cancellationToken)
    {
        var includes = new Expression<Func<Story, object>>[] {
            _ => _.StoryFandomLinks,
            _ => _.StoryFandomLinks.Select(_ => _.Fandom)
        };

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
            var refreshDate = _dateService.Now.AddDays(-1);// TODO: Configure/options

            var stories = await _repository.QueryEntities<Story>()
                .Include(_ => _.StoryFandomLinks)
                .ThenInclude(_ => _.Fandom)
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
}
