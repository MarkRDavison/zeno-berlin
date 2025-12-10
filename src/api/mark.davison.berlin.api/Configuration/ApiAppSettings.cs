namespace mark.davison.berlin.api.Configuration;

public class ApiAppSettings : IRootAppSettings
{
    public string SECTION => "BERLIN";

    public AuthenticationSettings AUTHENTICATION { get; set; } = new();
    public RedisSettings REDIS { get; set; } = new();
    public DatabaseAppSettings DATABASE { get; set; } = new();
    public Ao3Config AO3 { get; set; } = new();
    public JobAppSettings JOBS { get; set; } = new();
    public NotificationSettings NOTIFICATIONS { get; set; } = new();
    public bool PRODUCTION_MODE { get; set; }
}