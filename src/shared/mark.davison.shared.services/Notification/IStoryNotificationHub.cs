namespace mark.davison.shared.services.Notification;
// TODO: Rename and Move to common
public interface IStoryNotificationHub
{
    Task<Response> SendNotification(string message);
}
