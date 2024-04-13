namespace mark.davison.shared.services.Notification.Console;

public class ConsoleNotificationService : IConsoleNotificationService
{
    public ConsoleNotificationService(IOptions<ConsoleNotificationSettings> options)
    {
        Settings = options.Value;
    }

    public StoryNotificationSettings Settings { get; }

    public Task<Response> SendNotification(string message)
    {
        throw new NotImplementedException();
    }
}
