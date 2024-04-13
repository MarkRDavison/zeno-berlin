namespace mark.davison.shared.services.Notification;

public abstract class StoryNotificationSettings : IAppSettings
{
    public abstract string SECTION { get; }
    public abstract bool ENABLED { get; set; }
}
