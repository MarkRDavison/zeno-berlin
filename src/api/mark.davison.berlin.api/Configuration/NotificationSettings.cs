namespace mark.davison.berlin.api.Configuration;

public sealed class NotificationSettings : IAppSettings
{
    public string SECTION => "NOTIFICATIONS";

    public MatrixNotificationSettings MATRIX { get; set; } = new();
    public ConsoleNotificationSettings CONSOLE { get; set; } = new();

}
