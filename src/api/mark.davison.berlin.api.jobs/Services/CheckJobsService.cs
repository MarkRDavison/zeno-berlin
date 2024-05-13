namespace mark.davison.berlin.api.jobs.Services;

public class CheckJobsService : ICheckJobsService
{
    private readonly ILockService _lockService;
    private readonly IDateService _dateService;
    private readonly AppSettings _appSettings;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public CheckJobsService(
        ILockService lockService,
        IDateService dateService,
        IOptions<AppSettings> options,
        IServiceScopeFactory serviceScopeFactory)
    {
        _lockService = lockService;
        _dateService = dateService;
        _appSettings = options.Value;
        _serviceScopeFactory = serviceScopeFactory;
    }

    // TODO: What to do if you have a crash in the middle of a job? restarting????
    // Will need an admin page for outstanding jobs. A NEW APP :)
    //  - Ability to remove job history etc
    //  - Trigger job to restart
    //  - Remove failed jobs/edit and restart????

    public async Task<(bool LockAcquired, Job? job)> CheckForAvailableJob(HashSet<Guid> ignoreIds, CancellationToken cancellationToken)
    {
        var BerlinSyncLockKey = "BERLIN_LOCK" + (_appSettings.PRODUCTION_MODE ? "_PROD" : "_DEV");
        var BerlinSyncLockValue = "LOCKED";

        Job? availableJob = null;

        using var scope = _serviceScopeFactory.CreateScope();

        var idsToIgnore = ignoreIds.ToList();

        await using (var lockInfo = await _lockService.LockAsync(
            BerlinSyncLockKey,
            BerlinSyncLockValue,
            TimeSpan.FromSeconds(100)))
        {
            if (!lockInfo.LockAcquired)
            {
                lockInfo.AcknowledgeLockFailed();
                return (false, null);
            }

            var repository = scope.ServiceProvider.GetRequiredService<IRepository>();

            await using (repository.BeginTransaction())
            {
                var jobs = await repository.QueryEntities<Job>()
                    .Include(_ => _.ContextUser)
                    .Where(_ =>
                        !idsToIgnore.Contains(_.Id) &&
                        (_.Status == JobStatusConstants.Submitted && _.PerformerId == string.Empty))
                    .OrderBy(_ => _.SubmittedAt)
                    .Take(1)
                    .ToListAsync(cancellationToken); // TODO: EF testing stuff/FirstOrDefaultAsync not working


                if (jobs.FirstOrDefault() is Job job)
                {
                    Console.WriteLine("Found job: {0} with status {1}", job.Id, job.Status);

                    job.Status = JobStatusConstants.Selected;
                    job.PerformerId = Environment.MachineName;
                    job.SelectedAt = _dateService.Now;
                    job.LastModified = job.SelectedAt;

                    availableJob = await repository.UpsertEntityAsync(job, cancellationToken);
                }
            }
        }

        return (true, availableJob);
    }
}
