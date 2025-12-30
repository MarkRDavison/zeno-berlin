namespace mark.davison.berlin.shared.server.services.Helpers;

internal class LockDisposable : ILockDisposable
{
    private readonly IRedisLockDisposable _redisLockDisposable;
    public LockDisposable(IRedisLockDisposable redisLockDisposable)
    {
        _redisLockDisposable = redisLockDisposable;
    }
    public bool LockAcquired => _redisLockDisposable.LockAcquired;

    public void AcknowledgeLockFailed()
    {
        _redisLockDisposable.AcknowledgeLockFailed();
    }

    public async ValueTask DisposeAsync()
    {
        await _redisLockDisposable.DisposeAsync();
    }
}

public class LockService : ILockService
{
    private readonly IRedisService _redisService;

    public LockService(IRedisService redisService)
    {
        _redisService = redisService;
    }

    public Task<ILockDisposable> LockAsync(string key, string value) => LockAsync(key, value, TimeSpan.FromMinutes(1));

    public async Task<ILockDisposable> LockAsync(string key, string value, TimeSpan expiry)
    {
        var redisLockDisposable = await _redisService.LockAsync(key, value, expiry);

        return new LockDisposable(redisLockDisposable);
    }
}
