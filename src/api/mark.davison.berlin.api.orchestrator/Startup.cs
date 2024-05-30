namespace mark.davison.berlin.api.orchestrator;

public class Startup
{
    public IConfiguration Configuration { get; }

    public AppSettings AppSettings { get; set; } = null!;

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        AppSettings = services.ConfigureSettingsServices<AppSettings>(Configuration);
        if (AppSettings == null) { throw new InvalidOperationException(); }

        Console.WriteLine(AppSettings.DumpAppSettings(AppSettings.PRODUCTION_MODE));

        services
            .AddLogging()
            .AddDatabase<BerlinDbContext>(AppSettings.PRODUCTION_MODE, AppSettings.DATABASE, typeof(SqliteContextFactory), typeof(PostgresContextFactory))
            .AddCoreDbContext<BerlinDbContext>()
            .AddSingleton<IDateService>(new DateService(DateService.DateMode.Utc))
            .AddRedis(AppSettings.REDIS, "BERLIN_JOBS", AppSettings.PRODUCTION_MODE)
            .AddSingleton<IRedisService, RedisService>()
            .AddHostedService<HostedService>()
            .UseSharedServerServices(true);

        services.AddCronJob<CheckJobsCron>(_ =>
        {
            _.CronExpression = AppSettings.JOB_CHECK_RATE;
            _.TimeZoneInfo = AppSettings.CRON_TIMEZONE == "LOCAL" ? TimeZoneInfo.Local : TimeZoneInfo.Utc;
        });

        services.AddCronJob<MonthlyNotificationsCron>(_ =>
        {
            _.CronExpression = AppSettings.MONTHLY_STORY_NOTIFICATIONS_RATE;
            _.TimeZoneInfo = AppSettings.CRON_TIMEZONE == "LOCAL" ? TimeZoneInfo.Local : TimeZoneInfo.Utc;
        });

        services.AddCronJob<UpdateStoriesCron>(_ =>
        {
            _.CronExpression = AppSettings.STORY_UPDATE_RATE;
            _.TimeZoneInfo = AppSettings.CRON_TIMEZONE == "LOCAL" ? TimeZoneInfo.Local : TimeZoneInfo.Utc;
        });

        // TODO: Admin dashboard to fire these manually?
    }
}
