namespace mark.davison.berlin.api.queries.Scenarios.StartupQuery;

public sealed class StartupQueryHandler : ValidateAndProcessQueryHandler<StartupQueryRequest, StartupQueryResponse>
{
    public StartupQueryHandler(
        IQueryProcessor<StartupQueryRequest, StartupQueryResponse> processor
    ) : base(
        processor)
    {
    }
}
