using mark.davison.berlin.shared.constants;

namespace mark.davison.berlin.api.jobs.Services;

public class CheckJobsService : ICheckJobsService
{
    private readonly IRedisService _redisService;
    private readonly IDateService _dateService;
    private readonly AppSettings _appSettings;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public CheckJobsService(
        IRedisService redisService,
        IDateService dateService,
        IOptions<AppSettings> options,
        IServiceScopeFactory serviceScopeFactory)
    {
        _redisService = redisService;
        _dateService = dateService;
        _appSettings = options.Value;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task<(bool LockAcquired, Job? job)> CheckForAvailableJob(HashSet<Guid> ignoreIds, CancellationToken cancellationToken)
    {
        var BerlinSyncLockKey = "BERLIN_LOCK" + (_appSettings.PRODUCTION_MODE ? "_PROD" : "_DEV");
        var BerlinSyncLockValue = "LOCKED";

        Job? availableJob = null;

        using var scope = _serviceScopeFactory.CreateScope();

        var idsToIgnore = ignoreIds.ToList();

        await using (var lockInfo = await _redisService.LockAsync(
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
                        (_.Status == JobStatusConstants.Submitted && _.PerformerId == string.Empty) ||
                        (_.Status == JobStatusConstants.Selected && _.PerformerId == Environment.MachineName))
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
