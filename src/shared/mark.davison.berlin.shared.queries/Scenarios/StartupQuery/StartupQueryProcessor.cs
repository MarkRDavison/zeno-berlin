namespace mark.davison.berlin.shared.queries.Scenarios.StartupQuery;

public sealed class StartupQueryProcessor : IQueryProcessor<StartupQueryRequest, StartupQueryResponse>
{
    private readonly IDbContext<BerlinDbContext> _dbContext;

    public StartupQueryProcessor(IDbContext<BerlinDbContext> dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<StartupQueryResponse> ProcessAsync(StartupQueryRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var updateTypes = await _dbContext
            .Set<UpdateType>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return new StartupQueryResponse
        {
            Value = new()
            {
                UpdateTypes = [.. updateTypes.Select(_ => new UpdateTypeDto { Id = _.Id, Description = _.Description })]
            }
        };
    }
}
