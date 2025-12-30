namespace mark.davison.berlin.api.orchestrator.tests.Cron;

public sealed class CheckJobsCronTests
{
    private readonly Mock<IDistributedPubSub> _distributedPubSub;
    private readonly Mock<IServiceScopeFactory> _serviceScopeFactory;
    private readonly Mock<ILogger<CheckJobsCron>> _logger;
    private readonly OrchestratorAppSettings _appSettings;
    private readonly IDbContext<BerlinDbContext> _dbContext;

    private readonly CheckJobsCron _cron;

    public CheckJobsCronTests()
    {
        _distributedPubSub = new();
        _serviceScopeFactory = new();
        _logger = new();
        _appSettings = new();

        _dbContext = DbContextHelpers.CreateInMemory<BerlinDbContext>(_ => new(_));

        var scope = new Mock<IServiceScope>();

        _serviceScopeFactory
            .Setup(_ => _.CreateScope())
            .Returns(scope.Object);

        scope
            .Setup(_ => _.ServiceProvider)
            .Returns(new ServiceCollection()
                .AddSingleton(_dbContext)
                .BuildServiceProvider());

        _cron = new CheckJobsCron(
            new ScheduleConfig<CheckJobsCron>
            {
                CronExpression = "* * * * *",
                TimeZoneInfo = TimeZoneInfo.Local
            },
            _distributedPubSub.Object,
            _serviceScopeFactory.Object,
            _logger.Object,
            Options.Create(_appSettings));
    }

    [Test]
    public async Task DoWork_WhenNoJobs_DoesNothing()
    {
        await _cron.DoWork(CancellationToken.None);

        _distributedPubSub
            .Verify(
                _ => _.TriggerNotificationAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
    }

    [Test]
    public async Task DoWork_WhenJobs_TriggersPubSubNotification()
    {
        await _dbContext.AddAsync(new Job
        {
            Id = Guid.NewGuid(),
            Created = DateTime.UtcNow,
            LastModified = DateTime.UtcNow,
            UserId = Guid.NewGuid()
        }, CancellationToken.None);
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        await _cron.DoWork(CancellationToken.None);

        _distributedPubSub
            .Verify(
                _ => _.TriggerNotificationAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [Test]
    public async Task DoWork_WhenOnlyCompletedJobs_DoesNothing()
    {
        await _dbContext.AddAsync(new Job
        {
            Id = Guid.NewGuid(),
            Created = DateTime.UtcNow,
            LastModified = DateTime.UtcNow,
            UserId = Guid.NewGuid(),
            Status = JobStatusConstants.Complete
        }, CancellationToken.None);
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        await _cron.DoWork(CancellationToken.None);

        _distributedPubSub
            .Verify(
                _ => _.TriggerNotificationAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
    }
}
