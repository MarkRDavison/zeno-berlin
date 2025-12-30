namespace mark.davison.berlin.shared.models.dto.Scenarios.Commands.MonthlyNotifications;

[PostRequest(Path = "monthly-notifications-command")]
public sealed class MonthlyNotificationsCommandRequest : ICommand<MonthlyNotificationsCommandRequest, MonthlyNotificationsCommandResponse>
{
}
