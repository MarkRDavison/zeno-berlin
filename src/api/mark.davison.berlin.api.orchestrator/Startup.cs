namespace mark.davison.berlin.api.orchestrator;

public class Startup
{
    public OrchestratorAppSettings AppSettings { get; set; } = default!;
    public IConfiguration Configuration { get; }
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        AppSettings = services.BindAppSettings(Configuration);

        services
            .AddCors(o =>
            {
                o.AddDefaultPolicy(builder =>
                {
                    builder
                        .SetIsOriginAllowed(_ => true)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            })
            .AddLogging()
            // TODO: Does this need to know about the migration assemblies?
            .AddDatabase<BerlinDbContext>(AppSettings.PRODUCTION_MODE, AppSettings.DATABASE, typeof(SqliteContextFactory))
            .AddCoreDbContext<BerlinDbContext>()
            .AddServerCore()
            .AddRedis(AppSettings.REDIS, AppSettings.REDIS.INSTANCE_NAME + (AppSettings.PRODUCTION_MODE ? "_prod_" : "_dev_"))
            .UseLockPubSub(!string.IsNullOrEmpty(AppSettings.REDIS.HOST))
            .AddHealthCheckServices<ApplicationHealthStateHostedService>();

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
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app
            .UseHttpsRedirection()
            .UseRouting()
            .UseEndpoints(_ =>
            {
                _.MapCommonHealthChecks();
            });
    }
}
