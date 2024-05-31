namespace mark.davison.berlin.shared.queries.Scenarios.PotentialStoryList;

public sealed class PotentialStoryListQueryProcessor : IQueryProcessor<PotentialStoryListQueryRequest, PotentialStoryListQueryResponse>
{
    private readonly IDbContext _dbContext;

    public PotentialStoryListQueryProcessor(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PotentialStoryListQueryResponse> ProcessAsync(PotentialStoryListQueryRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var potentialStories = await _dbContext.Set<PotentialStory>()
                .Where(_ => _.UserId == currentUserContext.CurrentUser.Id)
                .Select(_ => new PotentialStoryDto
                {
                    Id = _.Id,
                    Address = _.Address,
                    Name = _.Name,
                    Summary = _.Summary
                })
                .ToListAsync(cancellationToken);

        return new PotentialStoryListQueryResponse
        {
            Value = potentialStories
        };
    }
}
