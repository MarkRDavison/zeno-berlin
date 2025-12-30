namespace mark.davison.berlin.api.queries.Scenarios.FandomList;

public sealed class FandomListQueryHandler : ValidateAndProcessQueryHandler<FandomListQueryRequest, FandomListQueryResponse>
{
    public FandomListQueryHandler(
        IQueryProcessor<FandomListQueryRequest, FandomListQueryResponse> processor
    ) : base(
        processor)
    {
    }
}
