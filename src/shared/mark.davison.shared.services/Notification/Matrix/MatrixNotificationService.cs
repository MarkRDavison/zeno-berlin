namespace mark.davison.shared.services.Notification.Matrix;

public class MatrixNotificationService : IMatrixNotificationService
{
    public MatrixNotificationService(IOptions<MatrixNotificationSettings> options)
    {
        Settings = options.Value;
    }

    public StoryNotificationSettings Settings { get; }

    public Task<Response> SendNotification(string message)
    {
        throw new NotImplementedException();
    }
}
