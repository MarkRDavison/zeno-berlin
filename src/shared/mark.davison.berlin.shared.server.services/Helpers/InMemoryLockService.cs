namespace mark.davison.berlin.shared.server.services.Helpers;

public class InMemoryLockService : ILockService
{
    public Task<ILockDisposable> LockAsync(string key, string value)
    {
        throw new NotImplementedException();
    }

    public Task<ILockDisposable> LockAsync(string key, string value, TimeSpan expiry)
    {
        throw new NotImplementedException();
    }
}
