namespace mark.davison.berlin.api.Configuration;

public class AppSettings : IAppSettings
{
    public string SECTION => "BERLIN";

    public AuthAppSettings AUTH { get; set; } = new();
    public DatabaseAppSettings DATABASE { get; set; } = new();
    public RedisAppSettings REDIS { get; set; } = new();
    public bool PRODUCTION_MODE { get; set; }
}
