namespace mark.davison.berlin.shared.server.services.Helpers;

public interface ILockDisposable : IAsyncDisposable
{
    bool LockAcquired { get; }
    void AcknowledgeLockFailed();
}

public interface ILockService
{

    Task<ILockDisposable> LockAsync(string key, string value);
    Task<ILockDisposable> LockAsync(string key, string value, TimeSpan expiry);
}
