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
            .UseDatabase<BerlinDbContext>(AppSettings.PRODUCTION_MODE, AppSettings.DATABASE, typeof(SqliteContextFactory), typeof(PostgresContextFactory));

        services
            .AddScoped<IRepository>(_ =>
                new BerlinRepository(
                    _.GetRequiredService<IDbContextFactory<BerlinDbContext>>(),
                    _.GetRequiredService<ILogger<BerlinRepository>>())
                )
            .AddScoped<IReadonlyRepository>(_ => _.GetRequiredService<IRepository>())
            .AddSingleton<IDateService>(new DateService(DateService.DateMode.Utc));


        var config = new ConfigurationOptions
        {
            EndPoints = { AppSettings.REDIS.HOST + ":" + AppSettings.REDIS.PORT },
            Password = AppSettings.REDIS.PASSWORD
        };

        IConnectionMultiplexer redis = ConnectionMultiplexer.Connect(config);
        services
            .AddStackExchangeRedisCache(_ =>
            {
                _.InstanceName = "BERLIN_JOBS_" + (AppSettings.PRODUCTION_MODE ? "prod_" : "dev_");
                _.Configuration = redis.Configuration;
            })
            .AddSingleton(redis)
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
