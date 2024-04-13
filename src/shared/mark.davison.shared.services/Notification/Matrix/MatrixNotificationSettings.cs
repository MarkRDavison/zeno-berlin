namespace mark.davison.shared.services.Notification.Matrix;

public class MatrixNotificationSettings : StoryNotificationSettings
{
    public override string SECTION => "MATRIX";
    public override bool ENABLED { get; set; }
}
