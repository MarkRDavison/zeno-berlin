namespace mark.davison.shared.services.Notification.Console;

public class ConsoleNotificationService : IConsoleNotificationService
{
    private readonly ILogger<ConsoleNotificationService> _logger;
    private readonly ConsoleNotificationSettings _settings;
    public ConsoleNotificationService(
        IOptions<ConsoleNotificationSettings> options,
        ILogger<ConsoleNotificationService> logger)
    {
        _settings = options.Value;
        _logger = logger;
    }

    public StoryNotificationSettings Settings => _settings;

    public Task<Response> SendNotification(string message)
    {
        _logger.Log(_settings.LOGLEVEL, message);
        return Task.FromResult(new Response());
    }
}
