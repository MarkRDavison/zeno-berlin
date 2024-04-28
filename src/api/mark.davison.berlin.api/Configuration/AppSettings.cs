namespace mark.davison.berlin.api.Configuration;

public class AppSettings : IAppSettings
{
    public string SECTION => "BERLIN";

    public AuthAppSettings AUTH { get; set; } = new();
    public DatabaseAppSettings DATABASE { get; set; } = new();
    public RedisAppSettings REDIS { get; set; } = new();
    public Ao3Config AO3 { get; set; } = new();
    public NotificationSettings NOTIFICATIONS { get; set; } = new();
    public bool PRODUCTION_MODE { get; set; }
    public string STORY_UPDATE_CRON { get; set; } = "0 3 * * *"; // "0 6,18 * * *"  -> 6am/pm every day
    public int STORIES_PER_CRON_UPDATE { get; set; } = 10;
}
