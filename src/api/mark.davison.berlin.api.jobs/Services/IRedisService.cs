namespace mark.davison.berlin.api.jobs.Services;


public interface IRedisLockDisposable : IAsyncDisposable
{
    bool LockAcquired { get; }
    void AcknowledgeLockFailed();
}

public interface IRedisService
{
    Task<IRedisLockDisposable> LockAsync(string key, string value);
    Task<IRedisLockDisposable> LockAsync(string key, string value, TimeSpan expiry);

    Task SubscribeToKeyAsync(string key, string type, Func<Task> callback);
    void SubscribeToAdditionalKey(string key, string type, Func<Task> callback);
}
