namespace mark.davison.berlin.shared.models.dto.Scenarios.Queries.StoryList;

[GetRequest(Path = "story-list-query")]
public sealed class StoryListQueryRequest : IQuery<StoryListQueryRequest, StoryListQueryResponse>
{
}
