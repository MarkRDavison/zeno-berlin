namespace mark.davison.berlin.api.queries.Scenarios.AuthorList;

public sealed class AuthorListQueryProcessor : IQueryProcessor<AuthorListQueryRequest, AuthorListQueryResponse>
{
    private readonly IDbContext<BerlinDbContext> _dbContext;

    public AuthorListQueryProcessor(IDbContext<BerlinDbContext> dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AuthorListQueryResponse> ProcessAsync(AuthorListQueryRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var authors = await _dbContext
            .Set<Author>()
            .AsNoTracking()
                .Where(_ => _.UserId == currentUserContext.UserId)
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
