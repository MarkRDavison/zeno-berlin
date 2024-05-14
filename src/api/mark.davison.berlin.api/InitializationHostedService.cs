namespace mark.davison.berlin.api;

public sealed class InitializationHostedService : GenericApplicationHealthStateHostedService
{
    private readonly IBerlinDataSeeder _dataSeeder;
    private readonly IDbContextFactory<BerlinDbContext> _dbContextFactory;
    private readonly IOptions<AppSettings> _appSettings;

    public InitializationHostedService(
        IHostApplicationLifetime hostApplicationLifetime,
        IApplicationHealthState applicationHealthState,
        IBerlinDataSeeder dataSeeder,
        IDbContextFactory<BerlinDbContext> dbContextFactory,
        IOptions<AppSettings> appSettings
    ) : base(
        hostApplicationLifetime,
        applicationHealthState
    )
    {
        _dataSeeder = dataSeeder;
        _dbContextFactory = dbContextFactory;
        _appSettings = appSettings;
    }

    protected override async Task AdditionalStartAsync(CancellationToken cancellationToken)
    {
        var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        if (_appSettings.Value.PRODUCTION_MODE)
        {
            await dbContext.Database.MigrateAsync();
        }
        else
        {
            await dbContext.Database.EnsureDeletedAsync(cancellationToken);
            await dbContext.Database.EnsureCreatedAsync(cancellationToken);
        }

        await _dataSeeder.EnsureDataSeeded(cancellationToken);

        await base.AdditionalStartAsync(cancellationToken);
    }
}
