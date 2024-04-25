namespace mark.davison.shared.server.services.Helpers;

public interface IFandomService
{
    Task<List<Fandom>> GetOrCreateFandomsByExternalNames(List<string> externalNames, CancellationToken cancellationToken);
    Task<Fandom?> RetrieveFandomByExternalName(string externalName, CancellationToken cancellationToken);
}
