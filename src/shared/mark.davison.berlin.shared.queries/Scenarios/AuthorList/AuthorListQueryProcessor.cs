namespace mark.davison.berlin.shared.queries.Scenarios.AuthorList;

public sealed class AuthorListQueryProcessor : IQueryProcessor<AuthorListQueryRequest, AuthorListQueryResponse>
{
    private readonly IReadonlyRepository _repository;

    public AuthorListQueryProcessor(IReadonlyRepository repository)
    {
        _repository = repository;
    }

    public async Task<AuthorListQueryResponse> ProcessAsync(AuthorListQueryRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        await using (_repository.BeginTransaction())
        {
            var authors = await _repository.QueryEntities<Author>()
                .Where(_ => _.UserId == currentUserContext.CurrentUser.Id)
                .Select(_ => new AuthorDto // TODO: Helper
                {
                    AuthorId = _.Id,
                    ParentAuthorId = _.ParentAuthorId,
                    Name = _.Name,
                    IsUserSpecified = _.IsUserSpecified
                })
                .ToListAsync();

            return new AuthorListQueryResponse
            {
                Value = authors
            };
        }
    }
}
