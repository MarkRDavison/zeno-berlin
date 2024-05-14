namespace mark.davison.berlin.shared.queries.Scenarios.StartupQuery;

public sealed class StartupQueryHandler : ValidateAndProcessQueryHandler<StartupQueryRequest, StartupQueryResponse>
{
    public StartupQueryHandler(
        IQueryProcessor<StartupQueryRequest, StartupQueryResponse> processor
    ) : base(
        processor)
    {
    }
}
