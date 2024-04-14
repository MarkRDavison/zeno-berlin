namespace mark.davison.shared.services.Notification;

public class NotificationSettings : IAppSettings
{
    public string SECTION => "NOTIFICATIONS";
    public MatrixNotificationSettings MATRIX { get; set; } = new();
    public ConsoleNotificationSettings CONSOLE { get; set; } = new();
}
