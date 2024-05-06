namespace mark.davison.berlin.api.jobs.Configuration;

public class AppSettings : IAppSettings
{
    public string SECTION => "BERLIN";
    public DatabaseAppSettings DATABASE { get; set; } = new();
    public RedisAppSettings REDIS { get; set; } = new();
    public string JOB_CHECK_RATE { get; set; } = "0 * * * *";
    public string JOB_CHECK_EVENT_KEY { get; set; } = "jobeventkeyberlin";
    public bool PRODUCTION_MODE { get; set; }
    public string JOB_CHECK_EVENT_KEY_NAME => (JOB_CHECK_EVENT_KEY + (PRODUCTION_MODE ? "_PROD" : "_DEV")).ToUpperInvariant();
}
