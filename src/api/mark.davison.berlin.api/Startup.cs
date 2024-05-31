namespace mark.davison.berlin.api;

[UseCQRSServer(typeof(DtosRootType), typeof(CommandsRootType), typeof(QueriesRootType))]
public sealed class Startup
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
            .AddCors()
            .AddLogging()
            .AddJwtAuth(AppSettings.AUTH)
            .AddAuthorization()
            .AddEndpointsApiExplorer()
            .AddSwaggerGen()
            .AddHttpContextAccessor()
            .AddHealthCheckServices<InitializationHostedService>()
            .AddScoped<ICurrentUserContext, CurrentUserContext>()
            .AddEndpointsApiExplorer()
            .AddSwaggerGen()
            .AddDatabase<BerlinDbContext>(AppSettings.PRODUCTION_MODE, AppSettings.DATABASE, typeof(SqliteContextFactory), typeof(PostgresContextFactory))
            .AddCoreDbContext<BerlinDbContext>()
            .AddSingleton<IDateService>(new DateService(DateService.DateMode.Utc))
            .AddCQRSServer()
            .AddHttpClient()
            .AddHttpContextAccessor()
            .UseDataSeeders()
            .UseBerlinLogic(AppSettings.PRODUCTION_MODE)
            .UseSharedServices()
            .UseSharedServerServices(!string.IsNullOrEmpty(AppSettings.REDIS.HOST))
            .UseRateLimiter()
            .UseNotificationHub()
            .UseMatrixClient()
            .UseMatrixNotifications()
            .UseConsoleNotifications()
            .AddRedis(AppSettings.REDIS, AppSettings.SECTION, AppSettings.PRODUCTION_MODE);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseCors(builder =>
            builder
                .SetIsOriginAllowedToAllowWildcardSubdomains()
                .SetIsOriginAllowed(_ => true) // TODO: Config driven
                .AllowAnyMethod()
                .AllowCredentials()
                .AllowAnyHeader());

        app.UseHttpsRedirection();

        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app
            .UseMiddleware<RequestResponseLoggingMiddleware>()
            .UseRouting()
            .UseAuthentication()
            .UseAuthorization()
            .UseMiddleware<PopulateUserContextMiddleware>()
            .UseMiddleware<ValidateUserExistsInDbMiddleware>() // TODO: To common
            .UseEndpoints(endpoints =>
        {
            endpoints
                .MapHealthChecks()
                .MapGet<User>()
                .MapGetById<User>()
                .MapPost<User>()
                .MapCQRSEndpoints();

            if (!AppSettings.PRODUCTION_MODE)
            {
                endpoints.MapResetEndpoints();
            }
        });
    }
}
