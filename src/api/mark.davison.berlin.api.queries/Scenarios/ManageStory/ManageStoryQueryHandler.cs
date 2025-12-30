namespace mark.davison.berlin.api.queries.Scenarios.ManageStory;

public sealed class ManageStoryQueryHandler : ValidateAndProcessQueryHandler<ManageStoryQueryRequest, ManageStoryQueryResponse>
{
    public ManageStoryQueryHandler(
        IQueryProcessor<ManageStoryQueryRequest, ManageStoryQueryResponse> processor
    ) : base(
        processor)
    {
    }
}
