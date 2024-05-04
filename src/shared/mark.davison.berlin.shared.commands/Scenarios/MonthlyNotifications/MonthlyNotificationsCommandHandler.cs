namespace mark.davison.berlin.shared.commands.Scenarios.MonthlyNotifications;

public class MonthlyNotificationsCommandHandler : ValidateAndProcessCommandHandler<MonthlyNotificationsCommandRequest, MonthlyNotificationsCommandResponse>
{
    public MonthlyNotificationsCommandHandler(
        ICommandProcessor<MonthlyNotificationsCommandRequest, MonthlyNotificationsCommandResponse> processor
    ) : base(
        processor)
    {
    }
}
