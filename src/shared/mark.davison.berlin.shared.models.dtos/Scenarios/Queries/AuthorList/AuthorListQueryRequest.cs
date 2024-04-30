namespace mark.davison.berlin.shared.models.dtos.Scenarios.Queries.AuthorList;

[GetRequest(Path = "author-list-query")]
public class AuthorListQueryRequest : IQuery<AuthorListQueryRequest, AuthorListQueryResponse>
{
    public List<Guid> AuthorIds { get; set; } = [];
}
