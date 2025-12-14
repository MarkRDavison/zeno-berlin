namespace mark.davison.berlin.api.queries.Scenarios.StartupQuery;

public sealed class StartupQueryProcessor : IQueryProcessor<StartupQueryRequest, StartupQueryResponse>
{
    private readonly IDbContext<BerlinDbContext> _dbContext;
    private readonly IOptions<AuthenticationSettings> _authSettings;

    public StartupQueryProcessor(
        IDbContext<BerlinDbContext> dbContext,
        IOptions<AuthenticationSettings> authSettings)
    {
        _dbContext = dbContext;
        _authSettings = authSettings;
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
                EnabledAuthProviders = [.. _authSettings.Value.PROVIDERS.Select(_ => _.Name)],
                UpdateTypes = [.. updateTypes.Select(_ => new UpdateTypeDto { Id = _.Id, Description = _.Description })]
            }
        };
    }
}
