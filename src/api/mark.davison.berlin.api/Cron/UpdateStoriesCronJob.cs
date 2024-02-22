namespace mark.davison.berlin.api.Cron;

public class UpdateStoriesCronJob : CronJobService
{
    public UpdateStoriesCronJob(
        string cronExpression,
        TimeZoneInfo timeZoneInfo) : base(
        cronExpression,
        timeZoneInfo)
    {
    }

    public override Task DoWork(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
