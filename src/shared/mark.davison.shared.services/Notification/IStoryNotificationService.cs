namespace mark.davison.shared.services.Notification;

public interface IStoryNotificationService
{
    StoryNotificationSettings Settings { get; }
    Task<Response> SendNotification(string message);
}
