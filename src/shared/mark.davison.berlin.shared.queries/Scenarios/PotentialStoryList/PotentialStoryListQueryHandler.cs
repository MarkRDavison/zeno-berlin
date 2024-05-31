namespace mark.davison.berlin.shared.queries.Scenarios.PotentialStoryList;

public sealed class PotentialStoryListQueryHandler : ValidateAndProcessQueryHandler<PotentialStoryListQueryRequest, PotentialStoryListQueryResponse>
{
    public PotentialStoryListQueryHandler(
        IQueryProcessor<PotentialStoryListQueryRequest, PotentialStoryListQueryResponse> processor
    ) : base(
        processor)
    {
    }
}
