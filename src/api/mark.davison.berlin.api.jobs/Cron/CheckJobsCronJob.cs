namespace mark.davison.berlin.api.jobs.Cron;

public class CheckJobsCronJob : CronJobService
{
    private readonly ICheckJobsService _checkJobsService;

    public CheckJobsCronJob(
        IScheduleConfig<CheckJobsCronJob> scheduleConfig,
        ICheckJobsService checkJobsService
    ) : base(scheduleConfig.CronExpression, scheduleConfig.TimeZoneInfo)
    {
        _checkJobsService = checkJobsService;
    }

    public override Task DoWork(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
