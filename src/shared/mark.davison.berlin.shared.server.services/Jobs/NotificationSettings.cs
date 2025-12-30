namespace mark.davison.berlin.shared.server.services.Jobs;

public sealed class NotificationSettings : IAppSettings
{
    public MatrixNotificationSettings MATRIX { get; set; } = new();
    public ConsoleNotificationSettings CONSOLE { get; set; } = new();

}