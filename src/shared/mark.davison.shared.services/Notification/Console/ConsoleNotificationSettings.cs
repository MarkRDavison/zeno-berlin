namespace mark.davison.shared.services.Notification.Console;

public class ConsoleNotificationSettings : StoryNotificationSettings
{
    public override string SECTION => "CONSOLE";
    public override bool ENABLED { get; set; }
}
