namespace mark.davison.berlin.shared.server.services.Helpers;

public interface IAuthorService
{
    Task<List<Author>> GetOrCreateAuthorsByName(List<string> names, Guid siteId, CancellationToken cancellationToken);
    Task<Author?> RetrieveAuthorByName(string name, CancellationToken cancellationToken);
}
