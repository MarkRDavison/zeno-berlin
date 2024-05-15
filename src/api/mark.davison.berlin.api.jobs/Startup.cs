namespace mark.davison.berlin.api.jobs;

[UseCQRSServer(typeof(DtosRootType), typeof(CommandsRootType), typeof(QueriesRootType))]
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
            .ConfigureHealthCheckServices<InitializationHostedService>()
            .AddCors(options =>
                options.AddPolicy("AllowOrigin", _ => _
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .SetIsOriginAllowed(_ => true) // TODO: Config driven
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .AllowAnyHeader()
                    .Build()
                ));

        services
            .UseDatabase<BerlinDbContext>(AppSettings.PRODUCTION_MODE, AppSettings.DATABASE, typeof(SqliteContextFactory), typeof(PostgresContextFactory))
            .AddSingleton<ICheckJobsService, CheckJobsService>();

        services
            .AddScoped<IRepository>(_ =>
                new BerlinRepository(
                    _.GetRequiredService<IDbContextFactory<BerlinDbContext>>(),
                    _.GetRequiredService<ILogger<BerlinRepository>>())
                )
            .AddScoped<IReadonlyRepository>(_ => _.GetRequiredService<IRepository>());

        services.AddSingleton<IDateService>(new DateService(DateService.DateMode.Utc));
        services.UseValidation()
            .UseBerlinLogic()
            .UseSharedServices()
            .UseSharedServerServices(!string.IsNullOrEmpty(AppSettings.REDIS.HOST))
            .UseRateLimiter()
            .UseNotificationHub()
            .UseMatrixClient()
            .UseMatrixNotifications()
            .UseConsoleNotifications(); // TODO: Consolidate whatever you need to connect to db, run cqrs etc???

        services.UseCQRSServer();
        services
            .AddHttpClient()
            .AddHttpContextAccessor()
            .UseRateLimiter()
            .UseNotificationHub()
            .AddScoped<ICurrentUserContext>(_ => new CurrentUserContext
            {
                CurrentUser = new() // TODO: Fetch on startup???
                {
                    Id = Guid.Empty,
                    Email = "berlinsystem@markdavison.kiwi",
                    First = "Berlin",
                    Last = "System",
                    Username = "Berlin.System"
                }
            });

        if (!string.IsNullOrEmpty(AppSettings.REDIS.PASSWORD))
        {
            var config = new ConfigurationOptions
            {
                EndPoints = { AppSettings.REDIS.HOST + ":" + AppSettings.REDIS.PORT },
                Password = AppSettings.REDIS.PASSWORD
            };
            IConnectionMultiplexer redis = ConnectionMultiplexer.Connect(config);
            services.AddStackExchangeRedisCache(_ =>
            {
                _.InstanceName = "BERLIN_JOBS_" + (AppSettings.PRODUCTION_MODE ? "prod_" : "dev_");
                _.Configuration = redis.Configuration;
            });
            services.AddSingleton(redis);
            services.AddSingleton<IRedisService, RedisService>();
            services.AddSingleton<IJobOrchestrationService, JobOrchestrationService>();
            services.AddSingleton<ICheckJobsService, CheckJobsService>();
        }
    }


    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseCors("AllowOrigin");

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints
                .MapHealthChecks();

            endpoints.MapPost("/api/notify", (HttpContext context) =>
            {
                Console.WriteLine("Checking for jobs");
                return Results.Ok();
            });

            // TODO: Don't think we want this???
            // endpoints
            //     .ConfigureCQRSEndpoints();
        });
    }
}