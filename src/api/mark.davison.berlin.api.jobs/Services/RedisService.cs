using StackExchange.Redis;

namespace mark.davison.berlin.api.jobs.Services;

internal class RedisLockDisposable : IRedisLockDisposable
{
    private readonly IDatabase _database;
    private bool _lockFailureAcknowledged;
    private readonly string _key;
    private readonly string _value;

    public RedisLockDisposable(
        IDatabase database,
        bool lockAcquired,
        string key,
        string value)
    {
        _database = database;
        LockAcquired = lockAcquired;
        _key = key;
        _value = value;
    }

    public bool LockAcquired { get; }

    public void AcknowledgeLockFailed()
    {
        _lockFailureAcknowledged = true;
    }

    public async ValueTask DisposeAsync()
    {
        if (!LockAcquired && !_lockFailureAcknowledged)
        {
            throw new InvalidOperationException("Must acknowledge a lock failing to be acquired.");
        }

        if (LockAcquired && !await _database.LockReleaseAsync(_key, _value))
        {
            // Log warning???
        }
    }
}


public class RedisService : IRedisService
{
    private bool _subscriptionMade;
    private readonly IConnectionMultiplexer _redis;

    private readonly IDictionary<string, IDictionary<string, Func<Task>>> _callbacks;

    public RedisService(IConnectionMultiplexer redis)
    {
        _redis = redis;
        _callbacks = new Dictionary<string, IDictionary<string, Func<Task>>>();
    }

    static string GetKey(string channel)
    {
        var index = channel.IndexOf(':');

        if (index >= 0 && index < channel.Length - 1)
        {
            return channel[(index + 1)..];
        }

        return channel;
    }

    public async Task SubscribeToKeyAsync(string key, string type, Func<Task> callback)
    {
        if (!_subscriptionMade)
        {
            var subscriber = _redis.GetSubscriber();

            await subscriber.SubscribeAsync(
                RedisChannel.Pattern("__keyspace@0__:*"),
                async (channel, redisType) =>
                {
                    var redisKey = GetKey(channel);

                    if (_callbacks.TryGetValue(redisType, out var typeCallbacks))
                    {
                        if (typeCallbacks.TryGetValue(redisKey, out var keyCallback))
                        {
                            await keyCallback().ContinueWith(_ =>
                            {
                                if (_.IsFaulted && _.Exception != null)
                                {
                                    // TODO: Logger
                                    Console.WriteLine(_.Exception.Message);
                                    Console.WriteLine(_.Exception.StackTrace);
                                }
                            });
                        }
                    }
                });
            _subscriptionMade = true;
        }

        SubscribeToAdditionalKey(key, type, callback);
    }

    public void SubscribeToAdditionalKey(string key, string type, Func<Task> callback)
    {
        if (!_subscriptionMade)
        {
            throw new InvalidOperationException("Must subscribe before adding additional keys");
        }

        if (!_callbacks.ContainsKey(type))
        {
            _callbacks.Add(type, new Dictionary<string, Func<Task>>());
        }

        var typeCallbacks = _callbacks[type];
        if (!typeCallbacks.ContainsKey(key))
        {
            typeCallbacks.Add(key, callback);
        }
        else
        {
            typeCallbacks[key] = callback;
        }
    }

    public Task<IRedisLockDisposable> LockAsync(string key, string value) => LockAsync(key, value, TimeSpan.FromMinutes(1));

    public async Task<IRedisLockDisposable> LockAsync(string key, string value, TimeSpan expiry)
    {
        var database = _redis.GetDatabase();

        var locked = await database.LockTakeAsync(key, value, expiry);
        // TODO: Pass in expiry so can warn if you try to release after expiry???
        return new RedisLockDisposable(database, locked, key, value);
    }
}
