namespace mark.davison.berlin.shared.queries.Scenarios.StoryList;

public sealed class StoryListQueryHandler : ValidateAndProcessQueryHandler<StoryListQueryRequest, StoryListQueryResponse>
{
    public StoryListQueryHandler(
        IQueryProcessor<StoryListQueryRequest, StoryListQueryResponse> processor
    ) : base(
        processor)
    {
    }
}
