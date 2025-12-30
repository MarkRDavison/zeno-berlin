namespace mark.davison.berlin.shared.models.dto.Scenarios.Queries.PotentialStoryList;

[GetRequest(Path = "potential-story-list-query")]
public sealed class PotentialStoryListQueryRequest : IQuery<PotentialStoryListQueryRequest, PotentialStoryListQueryResponse>
{
}
