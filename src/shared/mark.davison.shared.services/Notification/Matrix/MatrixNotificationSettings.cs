namespace mark.davison.shared.services.Notification.Matrix;

public class MatrixNotificationSettings : StoryNotificationSettings
{
    public override string SECTION => "MATRIX";
    public override bool ENABLED { get; set; }
    public string URL { get; set; } = "https://matrix-client.matrix.org";
    public string USERNAME { get; set; } = string.Empty;
    public string PASSWORD { get; set; } = string.Empty;
    public string ROOMID { get; set; } = string.Empty;
    public string SESSIONNAME { get; set; } = "fanfic-notification-bot";
}
