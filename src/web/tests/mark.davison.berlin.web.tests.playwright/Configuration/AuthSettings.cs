namespace mark.davison.berlin.web.tests.playwright.Configuration;

public class AuthSettings
{
    public string PROVIDER { get; set; } = string.Empty;
    public string USERNAME { get; set; } = string.Empty;
    public string PASSWORD { get; set; } = string.Empty;

    public void EnsureValid()
    {
        if (string.IsNullOrEmpty(PROVIDER))
        {
            throw new InvalidOperationException("PROVIDER must have a value");
        }

        if (string.IsNullOrEmpty(USERNAME))
        {
            throw new InvalidOperationException("USERNAME must have a value");
        }

        if (string.IsNullOrEmpty(PASSWORD))
        {
            throw new InvalidOperationException("PASSWORD must have a value");
        }
    }
}
