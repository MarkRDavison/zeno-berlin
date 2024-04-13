namespace mark.davison.shared.services.RateLimit;

public interface IRateLimitServiceFactory
{
    IRateLimitService CreateRateLimiter(TimeSpan delay);
}
