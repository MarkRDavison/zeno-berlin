namespace mark.davison.berlin.shared.commands.Scenarios.UpdateStories;

public class UpdateStoriesCommandProcessor : ICommandProcessor<UpdateStoriesRequest, UpdateStoriesResponse>
{
    private readonly IRepository _repository;
    private readonly IDateService _dateService;
    private readonly IServiceProvider _serviceProvider;

    public UpdateStoriesCommandProcessor(
        IRepository repository,
        IDateService dateService,
        IServiceProvider serviceProvider)
    {
        _repository = repository;
        _dateService = dateService;
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

                var site = await _repository.GetEntityAsync<Site>(g.Key, cancellationToken);
                if (site == null) { continue; }

                var storyInfoProcessor = _serviceProvider.GetKeyedService<IStoryInfoProcessor>(site.ShortName);
                if (storyInfoProcessor == null) { continue; }

                foreach (var story in g)
                {
                    if (cancellationToken.IsCancellationRequested) { break; }

                    await ProcessStory(site, story, _repository, storyInfoProcessor, cancellationToken);
                }
            }

            if (toUpdate.Any())
            {
                await _repository.UpsertEntitiesAsync(toUpdate);
            }
        }

        return new();
    }

    private async Task ProcessStory(Site site, Story story, IRepository repository, IStoryInfoProcessor storyInfoProcessor, CancellationToken cancellationToken)
    {
        var info = await storyInfoProcessor.ExtractStoryInfo(site.Address, cancellationToken);

        story.TotalChapters = info.TotalChapters;
        story.CurrentChapters = info.CurrentChapters;
        story.Complete = info.IsCompleted;
        story.Name = info.Name;
        story.LastModified = _dateService.Now;
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
                cancellationToken); // TODO: Allow repository to provide paging/max

            return stories.Take(max).ToList();
        }
    }
}
