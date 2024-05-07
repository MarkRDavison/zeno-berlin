namespace mark.davison.shared.server.services.Helpers;

public interface IDistributedPubSub
{
    Task TriggerNotificationAsync(string key, CancellationToken cancellationToken);

    Task SubscribeToKeyAsync(string key, Func<Task> callback);
}
