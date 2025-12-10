namespace mark.davison.berlin.shared.server.services.Helpers;

public sealed class SiteService : ISiteService
{
    private readonly IDbContext _dbContext;
    private readonly IServiceProvider _serviceProvider;

    public SiteService(
        IDbContext dbContext,
        IServiceProvider serviceProvider)
    {
        _dbContext = dbContext;
        _serviceProvider = serviceProvider;
    }

    public async Task<SiteInfo> DetermineSiteAsync(string address, Guid? siteId, CancellationToken cancellationToken)
    {
        address = address.Replace("http:", "https:");

        if (string.IsNullOrEmpty(address))
        {
            return new SiteInfo
            {
                Error = ValidationMessages.FormatMessageParameters(
                ValidationMessages.INVALID_PROPERTY,
                nameof(Story.Address))
            };
        }

        Site? site = null;

        var sites = await _dbContext
            .Set<Site>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        if (siteId == null)
        {
            site = sites.FirstOrDefault(_ => address.StartsWith(_.Address));

            if (site == null)
            {
                return new SiteInfo
                {
                    Error = ValidationMessages.UNSUPPORTED_SITE
                };
            }
        }
        else
        {
            site = sites.FirstOrDefault(_ => _.Id == siteId);
            if (site == null)
            {
                return new SiteInfo
                {
                    Error = ValidationMessages.FormatMessageParameters(
                    ValidationMessages.FAILED_TO_FIND_ENTITY,
                    nameof(Site))
                };
            }

            if (!address.StartsWith(site.Address))
            {
                return new SiteInfo
                {
                    Error = ValidationMessages.FormatMessageParameters(
                    ValidationMessages.SITE_STORY_MISMATCH)
                };
            }
        }

        var infoProcessor = _serviceProvider.GetKeyedService<IStoryInfoProcessor>(site.ShortName);

        if (infoProcessor == null)
        {
            return new SiteInfo
            {
                Error = ValidationMessages.FormatMessageParameters(
                ValidationMessages.UNSUPPORTED_SITE)
            };
        }

        var externalId = infoProcessor.ExtractExternalStoryId(address, site.Address);

        if (string.IsNullOrEmpty(externalId))
        {
            return new SiteInfo
            {
                Error = ValidationMessages.FormatMessageParameters(
                    ValidationMessages.INVALID_PROPERTY,
                    nameof(Story.Address))
            };
        }

        return new SiteInfo
        {
            Site = site,
            UpdatedAddress = infoProcessor.GenerateBaseStoryAddress(address, site.Address),
            ExternalId = externalId
        };
    }
}
