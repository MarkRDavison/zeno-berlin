namespace mark.davison.berlin.api.queries.Scenarios.AuthorList;

public sealed class AuthorListQueryHandler : ValidateAndProcessQueryHandler<AuthorListQueryRequest, AuthorListQueryResponse>
{
    public AuthorListQueryHandler(
        IQueryProcessor<AuthorListQueryRequest, AuthorListQueryResponse> processor
    ) : base(
        processor)
    {
    }
}
