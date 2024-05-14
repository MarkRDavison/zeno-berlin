namespace mark.davison.berlin.shared.models.dtos.Scenarios.Queries.FandomList;

[GetRequest(Path = "fandom-list-query")]
public sealed class FandomListQueryRequest : IQuery<FandomListQueryRequest, FandomListQueryResponse>
{
}
