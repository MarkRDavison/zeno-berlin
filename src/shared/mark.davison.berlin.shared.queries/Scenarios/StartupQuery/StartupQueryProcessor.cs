namespace mark.davison.berlin.shared.queries.Scenarios.StartupQuery;

public sealed class StartupQueryProcessor : IQueryProcessor<StartupQueryRequest, StartupQueryResponse>
{
    private readonly IReadonlyRepository _repository;

    public StartupQueryProcessor(IReadonlyRepository repository)
    {
        _repository = repository;
    }

    public async Task<StartupQueryResponse> ProcessAsync(StartupQueryRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        await using (_repository.BeginTransaction())
        {
            var updateTypes = await _repository.GetEntitiesAsync<UpdateType>(cancellationToken);

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
