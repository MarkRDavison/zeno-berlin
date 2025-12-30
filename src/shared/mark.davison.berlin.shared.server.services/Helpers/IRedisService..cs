namespace mark.davison.berlin.shared.server.services.Helpers;

public interface IRedisLockDisposable : IAsyncDisposable
{
    bool LockAcquired { get; }
    void AcknowledgeLockFailed();
}

public interface IRedisService
{
    Task SetValueAsync(string key, string value, CancellationToken cancellationToken);
    Task SetValueAsync(string key, string value, TimeSpan expiry, CancellationToken cancellationToken);

    Task<string?> GetStringValueAsync(string key, CancellationToken cancellationToken);

    Task<IRedisLockDisposable> LockAsync(string key, string value);
    Task<IRedisLockDisposable> LockAsync(string key, string value, TimeSpan expiry);

    Task SubscribeToKeyAsync(string key, string type, Func<Task> callback);
    void SubscribeToAdditionalKey(string key, string type, Func<Task> callback);
}
