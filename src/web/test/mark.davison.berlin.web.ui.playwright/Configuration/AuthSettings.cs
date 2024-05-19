namespace mark.davison.berlin.web.ui.playwright.Configuration;

public class AuthSettings
{
    public string Section => "AUTH";

    public string USERNAME { get; set; } = string.Empty;
    public string PASSWORD { get; set; } = string.Empty;

    public void EnsureValid()
    {
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
