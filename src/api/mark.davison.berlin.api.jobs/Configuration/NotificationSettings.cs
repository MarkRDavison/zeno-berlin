namespace mark.davison.berlin.api.jobs.Configuration;

public class NotificationSettings : IAppSettings
{
    public MatrixNotificationSettings MATRIX { get; set; } = new();
    public ConsoleNotificationSettings CONSOLE { get; set; } = new();
}
