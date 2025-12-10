namespace mark.davison.berlin.api.queries.Scenarios.FandomList;

public sealed class FandomListQueryProcessor : IQueryProcessor<FandomListQueryRequest, FandomListQueryResponse>
{
    private readonly IDbContext<BerlinDbContext> _dbContext;

    public FandomListQueryProcessor(IDbContext<BerlinDbContext> dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<FandomListQueryResponse> ProcessAsync(FandomListQueryRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var fandoms = await _dbContext
            .Set<Fandom>()
            .AsNoTracking()
            .Where(_ => _.UserId == currentUserContext.UserId)
            .Select(_ => new FandomDto // TODO: Helper
            {
                FandomId = _.Id,
                ParentFandomId = _.ParentFandomId,
                Name = _.Name,
                ExternalName = _.ExternalName,
                IsHidden = _.IsHidden,
                IsUserSpecified = _.IsUserSpecified
            })
            .ToListAsync();

        return new FandomListQueryResponse
        {
            Value = fandoms
        };
    }
}
