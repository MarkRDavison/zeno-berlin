namespace mark.davison.shared.services.Notification;

public class StoryNotificationHub : IStoryNotificationHub
{
    private readonly List<IStoryNotificationService> _notificationServices;

    public StoryNotificationHub(IEnumerable<IStoryNotificationService> notificationServices)
    {
        _notificationServices = notificationServices.ToList();
    }

    public async Task<Response> SendNotification(string message)
    {
        var response = new Response();

        foreach (var service in _notificationServices)
        {
            if (!service.Settings.ENABLED)
            {
                continue;
            }

            var serviceResponse = await service.SendNotification(message);

            response.Errors.AddRange(serviceResponse.Errors);
            response.Warnings.AddRange(serviceResponse.Warnings);
        }

        return response;
    }
}
