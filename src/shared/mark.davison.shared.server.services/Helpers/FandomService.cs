namespace mark.davison.shared.server.services.Helpers;

public class FandomService : IFandomService
{
    private readonly IRepository _repository;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly ILogger<FandomService> _logger;

    public FandomService(
        IRepository repository,
        ICurrentUserContext currentUserContext,
        ILogger<FandomService> logger)
    {
        _repository = repository;
        _currentUserContext = currentUserContext;
        _logger = logger;
    }

    public async Task<List<Fandom>> GetOrCreateFandomsByExternalNames(List<string> externalNames, CancellationToken cancellationToken)
    {
        var fandoms = new List<Fandom>();

        foreach (var externalName in externalNames)
        {
            var fandom = await RetrieveFandomByExternalName(externalName, cancellationToken);

            if (fandom == null)
            {
                fandom = new Fandom
                {
                    Id = Guid.NewGuid(),
                    UserId = _currentUserContext.CurrentUser.Id,
                    IsHidden = false,
                    IsUserSpecified = false,
                    ExternalName = externalName,
                    Name = externalName,
                    ParentFandomId = null
                };

                fandom = await _repository.UpsertEntityAsync(fandom, cancellationToken);
            }

            if (fandom == null)
            {
                _logger.LogWarning("Failed to retrieve or create fandom for external name '{0}'", externalName);
                continue;
            }

            fandoms.Add(fandom);
        }

        return fandoms;
    }
    public async Task<Fandom?> RetrieveFandomByExternalName(string externalName, CancellationToken cancellationToken)
    {
        // TODO: Cache... make a common thing or use validation context???
        return await _repository.GetEntityAsync<Fandom>(
            _ =>
                _.UserId == _currentUserContext.CurrentUser.Id &&
                _.ExternalName == externalName,
            cancellationToken);
    }
}
