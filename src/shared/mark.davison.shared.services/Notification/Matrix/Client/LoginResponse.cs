namespace mark.davison.shared.services.Notification.Matrix.Client;

public class LoginResponse
{
    public string UserId { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string HomeServer { get; set; } = string.Empty;
    public string DeviceId { get; set; } = string.Empty;
}
