namespace mark.davison.shared.services.Notification.Matrix;

public static class MatrixConstants
{
    public const string HttpClientName = "MATRIX_NOTIFICATION_CLIENT";
    public const string LoginPath = "/_matrix/client/v3/login";
    public static string SendMessagePath(string roomId) => $"/_matrix/client/v3/rooms/{roomId}/send/m.room.message";
}
