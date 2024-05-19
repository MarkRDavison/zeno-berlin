namespace mark.davison.berlin.web.ui.playwright.Configuration;

public class AppSettings
{
    public string Section => "BERLIN";

    public AuthSettings AUTH { get; set; } = new();
    public EnvironmentSettings ENVIRONMENT { get; set; } = new();

    public void EnsureValid()
    {
        AUTH.EnsureValid();
        ENVIRONMENT.EnsureValid();
    }
}
