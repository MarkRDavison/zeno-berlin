namespace mark.davison.berlin.shared.queries.Scenarios.StartupQuery;

public sealed class StartupQueryHandler : IQueryHandler<StartupQueryRequest, StartupQueryResponse>
{
    private readonly IReadonlyRepository _repository;

    public StartupQueryHandler(IReadonlyRepository repository)
    {
        _repository = repository;
    }

    public async Task<StartupQueryResponse> Handle(StartupQueryRequest query, ICurrentUserContext currentUserContext, CancellationToken cancellation)
    {
        await using (_repository.BeginTransaction())
        {
            var updateTypes = await _repository.GetEntitiesAsync<UpdateType>(cancellation);

            return new StartupQueryResponse
            {
                Value = new()
                {
                    UpdateTypes = [.. updateTypes.Select(_ => new UpdateTypeDto { Id = _.Id, Description = _.Description })]
                }
            };
        }
    }
}
