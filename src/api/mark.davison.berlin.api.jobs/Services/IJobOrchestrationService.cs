namespace mark.davison.berlin.api.jobs.Services;

public interface IJobOrchestrationService
{
    Task RunAnyAvailableJobs(CancellationToken cancellationToken);

    Task<Job> PerformJob(Job job, CancellationToken cancellationToken);

    Task InitialiseJobMonitoring();
}
