namespace mark.davison.berlin.shared.logic.Settings;

public sealed class Ao3Config : IAppSettings
{
    public string SECTION => "AO3";

    public int RATE_DELAY { get; set; } = 3;
    public string SiteAddress { get; set; } = SiteConstants.ArchiveOfOurOwn_Address;
}
