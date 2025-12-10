namespace mark.davison.berlin.shared.models.dto.Scenarios.Queries.FandomList;

[GetRequest(Path = "fandom-list-query")]
public sealed class FandomListQueryRequest : IQuery<FandomListQueryRequest, FandomListQueryResponse>
{
}
