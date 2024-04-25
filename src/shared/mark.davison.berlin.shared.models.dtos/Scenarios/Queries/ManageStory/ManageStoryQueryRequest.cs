namespace mark.davison.berlin.shared.models.dtos.Scenarios.Queries.ManageStory;

[GetRequest(Path = "manage-story-query")]
public class ManageStoryQueryRequest : IQuery<ManageStoryQueryRequest, ManageStoryQueryResponse>
{
    public Guid StoryId { get; set; }
}
