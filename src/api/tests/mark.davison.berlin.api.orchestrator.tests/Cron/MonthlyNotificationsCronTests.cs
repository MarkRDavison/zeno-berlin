namespace mark.davison.berlin.api.orchestrator.tests.Cron;

public sealed class MonthlyNotificationsCronTests
{
    private readonly Mock<IDistributedPubSub> _distributedPubSub;
    private readonly Mock<IDateService> _dateService;
    private readonly Mock<IServiceScopeFactory> _serviceScopeFactory;
    private readonly Mock<ILogger<MonthlyNotificationsCron>> _logger;
    private readonly OrchestratorAppSettings _appSettings;
    private readonly IDbContext<BerlinDbContext> _dbContext;

    private readonly MonthlyNotificationsCron _cron;

    public MonthlyNotificationsCronTests()
    {
        _distributedPubSub = new();
        _dateService = new();
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

        _cron = new MonthlyNotificationsCron(
            new ScheduleConfig<MonthlyNotificationsCron>
            {
                CronExpression = "* * * * *",
                TimeZoneInfo = TimeZoneInfo.Local
            },
            _dateService.Object,
            _distributedPubSub.Object,
            _serviceScopeFactory.Object,
            _logger.Object,
            Options.Create(_appSettings));
    }

    [Test]
    public async Task DoWork_TriggersNotification()
    {
        await _cron.DoWork(CancellationToken.None);

        _distributedPubSub
            .Verify(
                _ => _.TriggerNotificationAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [Test]
    public async Task DoWork_CreatesJobRecord()
    {
        await _cron.DoWork(CancellationToken.None);

        var job = await _dbContext
            .Set<Job>()
            .AsNoTracking()
            .SingleAsync();

        await Assert.That(job).IsNotNull();
        await Assert.That(job.JobType).Contains(nameof(MonthlyNotificationsCommandRequest));
    }

}
