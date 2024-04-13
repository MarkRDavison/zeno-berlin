using Aqua.EnumerableExtensions;
using mark.davison.shared.services.Notification;
using System.Text;

namespace mark.davison.berlin.shared.commands.Scenarios.UpdateStories;

public class UpdateStoriesCommandProcessor : ICommandProcessor<UpdateStoriesRequest, UpdateStoriesResponse>
{
    private readonly ILogger<UpdateStoriesCommandProcessor> _logger;
    private readonly IRepository _repository;
    private readonly IDateService _dateService;
    private readonly IStoryNotificationHub _storyNotificationHub;
    private readonly IServiceProvider _serviceProvider;

    public UpdateStoriesCommandProcessor(
        ILogger<UpdateStoriesCommandProcessor> logger,
        IRepository repository,
        IDateService dateService,
        IStoryNotificationHub storyNotificationHub,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _repository = repository;
        _dateService = dateService;
        _storyNotificationHub = storyNotificationHub;
        _serviceProvider = serviceProvider;
    }

    public async Task<UpdateStoriesResponse> ProcessAsync(UpdateStoriesRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        await using (_repository.BeginTransaction())
        {
            var toUpdate = await GetStoriesToUpdate(request, _repository, cancellationToken);

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

                    await ProcessStory(site, story, storyInfoProcessor, cancellationToken);
                }
            }

            if (toUpdate.Any())
            {
                await _repository.UpsertEntitiesAsync(toUpdate);
            }
        }

        return new();
    }

    private async Task ProcessStory(Site site, Story story, IStoryInfoProcessor storyInfoProcessor, CancellationToken cancellationToken)
    {
        var info = await storyInfoProcessor.ExtractStoryInfo(site.Address, cancellationToken);

        if (story.TotalChapters != info.TotalChapters ||
            story.CurrentChapters != info.CurrentChapters ||
            story.Complete != info.IsCompleted ||
            story.Name != info.Name)
        {
            await ProcessNotification(site, story, info, cancellationToken);
        }

        story.TotalChapters = info.TotalChapters;
        story.CurrentChapters = info.CurrentChapters;
        story.Complete = info.IsCompleted;
        story.Name = info.Name;
        story.LastModified = _dateService.Now;
    }

    private async Task ProcessNotification(Site site, Story story, StoryInfoModel info, CancellationToken cancellationToken)
    {
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

        var response = await _storyNotificationHub.SendNotification(builder.ToString());

        response.Errors.ForEach(_ => _logger.LogError(_));
        response.Warnings.ForEach(_ => _logger.LogWarning(_));
    }

    private async Task<List<Story>> GetStoriesToUpdate(UpdateStoriesRequest request, IRepository repository, CancellationToken cancellationToken)
    {
        if (request.StoryIds.Any())
        {
            var stories = await _repository.GetEntitiesAsync<Story>(
                _ => request.StoryIds.Contains(_.Id),
                cancellationToken);
            // TODO: Override max???
            return stories;
        }
        else
        {
            int max = request.Amount <= 0 ? 2 : Math.Min(request.Amount, 10);
            var refreshDate = _dateService.Now.AddDays(-1);// TODO: Configure/optionbs

            var stories = await _repository.GetEntitiesAsync<Story>(
                _ =>
                    _.Complete == false &&
                    _.LastModified <= refreshDate,
                cancellationToken); // TODO: Allow repository to provide paging/max/orderby

            return stories.Take(max).ToList();
        }
    }
}
