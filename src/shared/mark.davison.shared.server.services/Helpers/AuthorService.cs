namespace mark.davison.shared.server.services.Helpers;

public sealed class AuthorService : IAuthorService
{
    private readonly IDbContext<BerlinDbContext> _dbContext;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IDateService _dateService;
    private readonly ILogger<AuthorService> _logger;
    private readonly IDictionary<string, Author> _createdAuthors;

    public AuthorService(
        IDbContext<BerlinDbContext> dbContext,
        ICurrentUserContext currentUserContext,
        IDateService dateService,
        ILogger<AuthorService> logger)
    {
        _dbContext = dbContext;
        _currentUserContext = currentUserContext;
        _dateService = dateService;
        _logger = logger;
        _createdAuthors = new Dictionary<string, Author>();
    }

    public async Task<List<Author>> GetOrCreateAuthorsByName(List<string> names, Guid siteId, CancellationToken cancellationToken)
    {
        var authors = new List<Author>();

        foreach (var name in names)
        {
            var author = await RetrieveAuthorByName(name, cancellationToken);
            if (author == null)
            {
                author = new Author
                {
                    Id = Guid.NewGuid(),
                    UserId = _currentUserContext.CurrentUser.Id,
                    SiteId = siteId,
                    Name = name,
                    ParentAuthorId = null,
                    Created = _dateService.Now,
                    LastModified = _dateService.Now
                };

                _createdAuthors.Add(name, author);

                var authorResult = await _dbContext.AddAsync(author, cancellationToken);
                author = authorResult?.Entity;
            }

            if (author == null)
            {
                _logger.LogWarning("Failed to retrieve or create author for name '{0}'", name);
                continue;
            }

            authors.Add(author);
        }

        return authors;
    }

    public async Task<Author?> RetrieveAuthorByName(string name, CancellationToken cancellationToken)
    {
        if (_createdAuthors.TryGetValue(name, out var author))
        {
            return author;
        }

        return await _dbContext
            .Set<Author>()
            .AsNoTracking()
            .SingleOrDefaultAsync(
            _ =>
                _.UserId == _currentUserContext.CurrentUser.Id &&
                _.Name == name,
            cancellationToken);
    }
}
