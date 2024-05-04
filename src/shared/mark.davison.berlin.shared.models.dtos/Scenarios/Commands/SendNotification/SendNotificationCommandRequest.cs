﻿namespace mark.davison.berlin.shared.models.dtos.Scenarios.Commands.SendNotification;

[PostRequest(Path = "send-notification-command")]
public class SendNotificationCommandRequest : ICommand<SendNotificationCommandRequest, SendNotificationCommandResponse>
{
    public string Message { get; set; } = string.Empty;
}
