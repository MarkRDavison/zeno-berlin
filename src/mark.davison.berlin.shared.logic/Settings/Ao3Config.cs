namespace mark.davison.berlin.shared.logic.Settings;

public sealed class Ao3Config : IAppSettings
{
    public string SECTION => "AO3";

    public int RATE_DELAY { get; set; } = 3;
    public int FAV_UPDATE_DELAY_HOURS { get; set; } = 12;
    public int NONFAV_UPDATE_DELAY_HOURS { get; set; } = 24;
    public string SITE_ADDRESS { get; set; } = SiteConstants.ArchiveOfOurOwn_Address;
    public string USER_AGENT { get; set; } = string.Empty;
}
