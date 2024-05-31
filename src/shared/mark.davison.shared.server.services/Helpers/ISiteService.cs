namespace mark.davison.shared.server.services.Helpers;

public sealed class SiteInfo
{
    public string Error { get; set; } = string.Empty;
    public string UpdatedAddress { get; set; } = string.Empty;
    public string ExternalId { get; set; } = string.Empty;
    public Site? Site { get; set; }

    [MemberNotNullWhen(true, nameof(Site))]
    public bool Valid => string.IsNullOrEmpty(Error) && Site != null;
}

public interface ISiteService
{
    Task<SiteInfo> DetermineSiteAsync(string address, Guid? siteId, CancellationToken cancellationToken);
}
