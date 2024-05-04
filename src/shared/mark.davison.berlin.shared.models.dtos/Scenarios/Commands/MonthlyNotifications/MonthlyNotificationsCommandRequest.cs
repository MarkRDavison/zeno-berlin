namespace mark.davison.berlin.shared.models.dtos.Scenarios.Commands.MonthlyNotifications;

[PostRequest(Path = "monthly-notifications-command")]
public class MonthlyNotificationsCommandRequest : ICommand<MonthlyNotificationsCommandRequest, MonthlyNotificationsCommandResponse>
{
}
