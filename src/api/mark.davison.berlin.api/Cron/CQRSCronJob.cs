namespace mark.davison.berlin.api.Cron;

public abstract class CQRSCronJob<TCronJobService, TRequest, TResponse> : CronJobService
    where TRequest : class, ICommand<TRequest, TResponse>, new()
    where TResponse : class, new()
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    protected CQRSCronJob(
        IScheduleConfig<TCronJobService> scheduleConfig,
        IServiceScopeFactory serviceScopeFactory
    ) : base(
        scheduleConfig.CronExpression,
        scheduleConfig.TimeZoneInfo)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected virtual Guid JobUserId => Guid.Empty;
    protected abstract TRequest CreateRequest();
    protected virtual Task HandleResponse(TResponse response) => Task.CompletedTask;

    public override async Task DoWork(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();

        var currentUserContext = scope.ServiceProvider.GetRequiredService<ICurrentUserContext>();
        var repo = scope.ServiceProvider.GetRequiredService<IReadonlyRepository>();

        await using (repo.BeginTransaction())
        {
            currentUserContext.CurrentUser = await repo.GetEntityAsync<User>(JobUserId, cancellationToken) ?? throw new InvalidOperationException("Job user was not found");
        }

        var handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<TRequest, TResponse>>();

        var request = CreateRequest();

        var response = await handler.Handle(request, currentUserContext, cancellationToken);

        await HandleResponse(response);
    }
}
