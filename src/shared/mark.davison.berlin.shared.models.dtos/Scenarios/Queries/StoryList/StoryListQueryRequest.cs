namespace mark.davison.berlin.shared.models.dtos.Scenarios.Queries.StoryList;

[GetRequest(Path = "story-list-query")]
public sealed class StoryListQueryRequest : IQuery<StoryListQueryRequest, StoryListQueryResponse>
{
}
