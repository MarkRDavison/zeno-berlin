namespace mark.davison.berlin.shared.models.dto.Scenarios.Queries.AuthorList;

[GetRequest(Path = "author-list-query")]
public sealed class AuthorListQueryRequest : IQuery<AuthorListQueryRequest, AuthorListQueryResponse>
{
}
