namespace mark.davison.shared.services.RateLimit;

public class RateLimitServiceFactory : IRateLimitServiceFactory
{
    private readonly IDateService _dateService;

    public RateLimitServiceFactory(IDateService dateService)
    {
        _dateService = dateService;
    }

    public IRateLimitService CreateRateLimiter(TimeSpan delay)
    {
        return new RateLimitService(delay.TotalSeconds, _dateService);
    }
}
