namespace mark.davison.berlin.shared.models.dtos.Scenarios.Queries.StoryList;

[GetRequest(Path = "story-list-query")]
public class StoryListQueryRequest : IQuery<StoryListQueryRequest, StoryListQueryResponse>
{
}
