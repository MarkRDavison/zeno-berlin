namespace mark.davison.berlin.shared.server.services.Helpers;

public class InMemoryDisutributedPubSub : IDistributedPubSub
{
    public Task SubscribeToKeyAsync(string key, Func<Task> callback)
    {
        throw new NotImplementedException();
    }

    public Task TriggerNotificationAsync(string key, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
