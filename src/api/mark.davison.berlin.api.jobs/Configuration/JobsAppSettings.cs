namespace mark.davison.berlin.api.jobs.Configuration;

public class JobsAppSettings : IRootAppSettings
{
    public string SECTION => "BERLIN";
    public bool PRODUCTION_MODE { get; set; }
    public DatabaseAppSettings DATABASE { get; set; } = new();
    public RedisSettings REDIS { get; set; } = new();
    public NotificationSettings NOTIFICATIONS { get; set; } = new();
    public JobSettings JOBS { get; set; } = new();
    public Ao3Config AO3 { get; set; } = new();
}
