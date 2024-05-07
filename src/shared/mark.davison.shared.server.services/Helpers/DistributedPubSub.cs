namespace mark.davison.shared.server.services.Helpers;

public class DistributedPubSub : IDistributedPubSub
{
    private readonly IRedisService _redisService;

    public DistributedPubSub(IRedisService redisService)
    {
        _redisService = redisService;
    }

    public async Task TriggerNotificationAsync(string key, CancellationToken cancellationToken)
    {
        await _redisService.SetValueAsync(key, "NOTIFICATION", TimeSpan.FromSeconds(5), cancellationToken);
    }

    public async Task SubscribeToKeyAsync(string key, Func<Task> callback)
    {
        await _redisService.SubscribeToKeyAsync(key, "set", callback);
    }
}
