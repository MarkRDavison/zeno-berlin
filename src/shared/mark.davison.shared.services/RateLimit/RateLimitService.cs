namespace mark.davison.shared.services.RateLimit;

public class RateLimitService : IRateLimitService
{
    private readonly double _delay;
    private DateTime _next;
    private readonly IDateService _dateService;

    public RateLimitService(
        double delay,
        IDateService dateService)
    {
        _dateService = dateService;
        _delay = delay;
        _next = _dateService.Now.AddSeconds(_delay);
    }

    public async Task Wait(CancellationToken cancellationToken)
    {
        if (_next > _dateService.Now)
        {
            _next = _dateService.Now.AddSeconds(_delay);
        }
        else
        {
            await Task.Delay(_dateService.Now.AddSeconds(_delay) - _next, cancellationToken);
            _next = _dateService.Now.AddSeconds(_delay);
        }
    }
}
