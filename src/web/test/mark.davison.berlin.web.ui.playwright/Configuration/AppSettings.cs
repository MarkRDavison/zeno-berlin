namespace mark.davison.berlin.web.ui.playwright.Configuration;

public class AppSettings
{
    public string Section => "BERLIN";

    public AuthSettings AUTH { get; set; } = new();
    public EnvironmentSettings ENVIRONMENT { get; set; } = new();
    public string APP_TITLE { get; set; } = "Fanfic";

    public void EnsureValid()
    {
        if (string.IsNullOrEmpty(APP_TITLE))
        {
            throw new InvalidOperationException("APP_TITLE must have a value");
        }
        AUTH.EnsureValid();
        ENVIRONMENT.EnsureValid();
    }
}
