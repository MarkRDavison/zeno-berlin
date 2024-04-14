namespace mark.davison.shared.services.Notification.Matrix;

public class MatrixNotificationService : IMatrixNotificationService
{
    private readonly MatrixNotificationSettings _settings;

    public MatrixNotificationService(IOptions<MatrixNotificationSettings> options)
    {
        _settings = options.Value;
    }

    public StoryNotificationSettings Settings => _settings;

    public Task<Response> SendNotification(string message)
    {
        throw new NotImplementedException();
    }
}
