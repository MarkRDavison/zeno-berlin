namespace mark.davison.berlin.api.queries.Scenarios.PotentialStoryList;

public sealed class PotentialStoryListQueryHandler : ValidateAndProcessQueryHandler<PotentialStoryListQueryRequest, PotentialStoryListQueryResponse>
{
    public PotentialStoryListQueryHandler(
        IQueryProcessor<PotentialStoryListQueryRequest, PotentialStoryListQueryResponse> processor
    ) : base(
        processor)
    {
    }
}
