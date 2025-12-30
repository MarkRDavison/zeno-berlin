namespace mark.davison.berlin.api.commands.Scenarios.SendNotification;

public sealed class SendNotificationCommandHandler : ICommandHandler<SendNotificationCommandRequest, SendNotificationCommandResponse>
{
    private readonly INotificationHub _notificationHub;
    private readonly ILogger<SendNotificationCommandHandler> _logger;
    private readonly IEnumerable<INotificationService> _notificationServices;

    public SendNotificationCommandHandler(
        INotificationHub notificationHub,
        ILogger<SendNotificationCommandHandler> logger,
        IEnumerable<INotificationService> notificationServices)
    {
        _notificationHub = notificationHub;
        _logger = logger;
        _notificationServices = notificationServices;
    }

    public async Task<SendNotificationCommandResponse> Handle(SendNotificationCommandRequest command, ICurrentUserContext currentUserContext, CancellationToken cancellation)
    {
        _logger.LogInformation("SendNotificationCommandHandler.notificationServices.Count: {0}", _notificationServices.Count());

        foreach (var notificationService in _notificationServices)
        {
            _logger.LogInformation(" - NotificationService.{0}.ENABLED: {1}", notificationService.Settings.SECTION, notificationService.Settings.ENABLED);
        }

        var response = await _notificationHub.SendNotification(new NotificationMessage { Message = command.Message });

        if (response.Errors.Any())
        {
            _logger.LogError(string.Join(", ", response.Errors));
        }
        else
        {
            _logger.LogInformation("SendNotificationCommandHandler no errors");
        }

        if (response.Warnings.Any())
        {
            _logger.LogWarning(string.Join(", ", response.Warnings));
        }
        else
        {
            _logger.LogInformation("SendNotificationCommandHandler no warnings");
        }

        return new()
        {
            Errors = [.. response.Errors],
            Warnings = [.. response.Warnings]
        };
    }
}
