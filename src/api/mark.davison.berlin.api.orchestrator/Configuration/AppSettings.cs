namespace mark.davison.berlin.api.orchestrator.Configuration;

public class AppSettings : IAppSettings
{
    public string SECTION => "BERLIN";
    public DatabaseAppSettings DATABASE { get; set; } = new();
    public RedisAppSettings REDIS { get; set; } = new();
    public string JOB_CHECK_EVENT_KEY { get; set; } = "jobeventkeyberlin";
    public string JOB_CHECK_RATE { get; set; } = "*/15 * * * *";
    public string MONTHLY_STORY_NOTIFICATIONS_RATE { get; set; } = "0 8 1,15 * *";
    public string STORY_UPDATE_RATE { get; set; } = "0 3 * * *";
    public bool PRODUCTION_MODE { get; set; }
    public string JOB_CHECK_EVENT_KEY_NAME => (JOB_CHECK_EVENT_KEY + (PRODUCTION_MODE ? "_PROD" : "_DEV")).ToUpperInvariant();
    public string CRON_TIMEZONE { get; set; } = "LOCAL";
}
