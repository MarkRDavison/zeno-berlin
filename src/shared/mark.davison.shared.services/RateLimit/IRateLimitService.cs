namespace mark.davison.shared.services.RateLimit;
// TODO: Move to common
public interface IRateLimitService
{
    public Task Wait(CancellationToken cancellationToken);
}
