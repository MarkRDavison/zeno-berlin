namespace mark.davison.berlin.api.jobs.Services;

public interface ICheckJobsService
{
    Task<(bool LockAcquired, Job? job)> CheckForAvailableJob(HashSet<Guid> ignoreIds, CancellationToken cancellationToken);
}
