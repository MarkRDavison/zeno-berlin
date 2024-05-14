namespace mark.davison.berlin.api.jobs.Configuration;

public sealed class AppSettings : IAppSettings
{
    public string SECTION => "BERLIN";
    public DatabaseAppSettings DATABASE { get; set; } = new();
    public RedisAppSettings REDIS { get; set; } = new();
    public NotificationSettings NOTIFICATIONS { get; set; } = new();
    public JobAppSettings JOBS { get; set; } = new();
    public bool PRODUCTION_MODE { get; set; }
}
