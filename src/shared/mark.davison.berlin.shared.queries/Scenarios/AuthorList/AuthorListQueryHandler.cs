namespace mark.davison.berlin.shared.queries.Scenarios.AuthorList;

public class AuthorListQueryHandler : IQueryHandler<AuthorListQueryRequest, AuthorListQueryResponse>
{
    private readonly IReadonlyRepository _repository;

    public AuthorListQueryHandler(IReadonlyRepository repository)
    {
        _repository = repository;
    }

    public async Task<AuthorListQueryResponse> Handle(AuthorListQueryRequest query, ICurrentUserContext currentUserContext, CancellationToken cancellation)
    {
        await using (_repository.BeginTransaction())
        {
            var authorsQuery = _repository.QueryEntities<Author>()
                .Where(_ => _.UserId == currentUserContext.CurrentUser.Id);

            if (query.AuthorIds.Any())
            {
                authorsQuery = authorsQuery.Where(_ => query.AuthorIds.Contains(_.Id));
            }

            var authors = await authorsQuery
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
