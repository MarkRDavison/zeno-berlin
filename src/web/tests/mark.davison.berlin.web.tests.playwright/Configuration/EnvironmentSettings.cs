namespace mark.davison.berlin.web.tests.playwright.Configuration;

public class EnvironmentSettings
{
    public string Section => "ENVIRONMENT";

    public string WEB_ORIGIN { get; set; } = string.Empty;
    public string BFF_ORIGIN { get; set; } = string.Empty;
    public string API_ORIGIN { get; set; } = string.Empty;
    public string STORY_URL { get; set; } = string.Empty;
    public string? TEMP_DIR { get; set; }


    public void EnsureValid()
    {
        if (string.IsNullOrEmpty(WEB_ORIGIN))
        {
            throw new InvalidOperationException("WEB_ORIGIN must have a value");
        }

        if (string.IsNullOrEmpty(BFF_ORIGIN))
        {
            throw new InvalidOperationException("BFF_ORIGIN must have a value");
        }

        if (string.IsNullOrEmpty(API_ORIGIN))
        {
            throw new InvalidOperationException("API_ORIGIN must have a value");
        }

        if (string.IsNullOrEmpty(STORY_URL))
        {
            throw new InvalidOperationException("STORY_URL must have a value");
        }
    }
}