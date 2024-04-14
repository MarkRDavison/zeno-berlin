namespace mark.davison.shared.services.Notification.Matrix.Client;

public interface IMatrixClient
{
    Task<Response> SendMessage(string roomId, string message);
}
