namespace mark.davison.berlin.api.queries.Scenarios.DashboardList;

public sealed class DashboardListQueryProcessor : IQueryProcessor<DashboardListQueryRequest, DashboardListQueryResponse>
{
    private readonly IDbContext<BerlinDbContext> _dbContext;

    public DashboardListQueryProcessor(IDbContext<BerlinDbContext> dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DashboardListQueryResponse> ProcessAsync(DashboardListQueryRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var response = new DashboardListQueryResponse
        {
            Value = []
        };

        // TODO: FIXME: This creates an absolutely awful auto generated query., like 8 story/story update from clauses
        var storyUpdates = await _dbContext
            .Set<StoryUpdate>()
            .AsNoTracking()
            .Include(_ => _.Story)
            .ThenInclude(_ => _!.StoryFandomLinks)
            .ThenInclude(_ => _.Fandom!)
            .ThenInclude(_ => _!.ParentFandom!)
            .Where(_ => _.UserId == currentUserContext.UserId && _.Story!.Favourite)
            .GroupBy(_ => _.StoryId)
            .Select(_ => new
            {
                StoryId = _.Key,
                Update = _.OrderByDescending(u => u.LastAuthored).First()
            })
            .ToListAsync(cancellationToken);

        foreach (var row in storyUpdates)
        {
            if (row.Update.Story == null) { continue; }
            var dto = new DashboardTileDto
            {
                StoryId = row.StoryId,
                Name = row.Update.Story.Name,
                CurrentChapters = row.Update.Story.CurrentChapters,
                TotalChapters = row.Update.Story.TotalChapters,
                ConsumedChapters = row.Update.Story.ConsumedChapters,
                Complete = row.Update.Story.Complete,
                Favourite = row.Update.Story.Favourite,
                LastChecked = row.Update.Story.LastChecked,
                LastAuthored = row.Update.LastAuthored,
                Fandoms = row.Update.Story.StoryFandomLinks.Select(_ => _.FandomId).Distinct().ToList()
            };
            response.Value.Add(dto);
        }

        return response;
    }
}
