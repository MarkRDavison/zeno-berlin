namespace mark.davison.berlin.api.queries.Scenarios.StoryList;

public sealed class StoryListQueryProcessor : IQueryProcessor<StoryListQueryRequest, StoryListQueryResponse>
{
    private readonly IDbContext<BerlinDbContext> _dbContext;

    public StoryListQueryProcessor(IDbContext<BerlinDbContext> dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<StoryListQueryResponse> ProcessAsync(StoryListQueryRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var storyUpdates = await _dbContext
            .Set<StoryUpdate>()
            .AsNoTracking()
            .Include(_ => _.Story!)
            .ThenInclude(_ => _!.StoryFandomLinks)
            .Include(_ => _.Story!)
            .ThenInclude(_ => _!.StoryAuthorLinks)
            .Where(_ => _.UserId == currentUserContext.UserId)
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
