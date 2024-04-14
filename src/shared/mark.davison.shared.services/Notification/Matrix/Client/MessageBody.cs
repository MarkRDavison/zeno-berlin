namespace mark.davison.shared.services.Notification.Matrix.Client;

public class MessageBody
{
    public MessageBody(TextMessageBody textMessageBody)
    {
        Msgtype = "m.text";
        Body = textMessageBody.Body;
    }

    public string Msgtype { get; }
    public string? Body { get; }
}
