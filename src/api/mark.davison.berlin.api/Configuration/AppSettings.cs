namespace mark.davison.berlin.api.Configuration;

public sealed class AppSettings : IAppSettings
{
    public string SECTION => "BERLIN";

    public AuthAppSettings AUTH { get; set; } = new();
    public DatabaseAppSettings DATABASE { get; set; } = new();
    public RedisAppSettings REDIS { get; set; } = new();
    public ClaimsAppSettings CLAIMS { get; set; } = new();
    public Ao3Config AO3 { get; set; } = new();
    public NotificationSettings NOTIFICATIONS { get; set; } = new();
    public JobAppSettings JOBS { get; set; } = new();
    public bool PRODUCTION_MODE { get; set; }
}
