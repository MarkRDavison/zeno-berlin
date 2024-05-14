namespace mark.davison.berlin.shared.queries.Scenarios.StoryList;

public sealed class StoryListQueryProcessor : IQueryProcessor<StoryListQueryRequest, StoryListQueryResponse>
{
    private readonly IReadonlyRepository _repository;

    public StoryListQueryProcessor(IReadonlyRepository repository)
    {
        _repository = repository;
    }
    public async Task<StoryListQueryResponse> ProcessAsync(StoryListQueryRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        await using (_repository.BeginTransaction())
        {

            var storyUpdates = await _repository.QueryEntities<StoryUpdate>()
                .Include(_ => _.Story!)
                .ThenInclude(_ => _!.StoryFandomLinks)
                .Include(_ => _.Story!)
                .ThenInclude(_ => _!.StoryAuthorLinks)
                .Where(_ => _.UserId == currentUserContext.CurrentUser.Id)
                .GroupBy(_ => _.StoryId)
                .Select(_ => new
                {
                    StoryId = _.Key,
                    Update = _.OrderByDescending(u => u.LastModified).First()
                })
                .ToListAsync();

            return new StoryListQueryResponse
            {
                Value = storyUpdates
                    .Where(_ => _.Update.Story != null)
                    .Select(_ =>
                    {
                        return new StoryRowDto
                        {
                            StoryId = _.StoryId,
                            Name = _.Update.Story!.Name,
                            CurrentChapters = _.Update.Story!.CurrentChapters,
                            TotalChapters = _.Update.Story!.TotalChapters,
                            ConsumedChapters = _.Update.Story!.ConsumedChapters,
                            IsComplete = _.Update.Story!.Complete,
                            IsFavourite = _.Update.Story!.Favourite,
                            UpdateTypeId = _.Update.Story!.UpdateTypeId,
                            Fandoms = [.. _.Update.Story.StoryFandomLinks.Select(sfl => sfl.FandomId)],
                            Authors = [.. _.Update.Story.StoryAuthorLinks.Select(sal => sal.AuthorId)],
                            LastAuthored = _.Update.Story.LastAuthored
                        };
                    })
                    .ToList()
            };
        }
    }
}
