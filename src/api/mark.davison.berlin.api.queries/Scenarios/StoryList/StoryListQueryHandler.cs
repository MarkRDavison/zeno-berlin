namespace mark.davison.berlin.api.queries.Scenarios.StoryList;

public sealed class StoryListQueryHandler : ValidateAndProcessQueryHandler<StoryListQueryRequest, StoryListQueryResponse>
{
    public StoryListQueryHandler(
        IQueryProcessor<StoryListQueryRequest, StoryListQueryResponse> processor
    ) : base(
        processor)
    {
    }
}
