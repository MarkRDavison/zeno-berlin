namespace mark.davison.berlin.api;

[UseCQRSServer]
public class Startup(IConfiguration Configuration)
{
    public ApiAppSettings AppSettings { get; set; } = new();

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
            .AddSingleton<IDataSeeder>(_ => new BerlinDataSeeder(
                _.GetRequiredService<IDateService>(),
                _.GetRequiredService<IServiceScopeFactory>(),
                AppSettings.PRODUCTION_MODE))
            .AddAuthorization()
            .AddJwtAuthentication<BerlinDbContext>(AppSettings.AUTHENTICATION)
            .AddCoreDbContext<BerlinDbContext>()
            .AddHealthCheckServices<ApplicationHealthStateHostedService>()
            .UseSharedServerServices(!string.IsNullOrEmpty(AppSettings.REDIS.HOST))
            .UseBerlinLogic(AppSettings.PRODUCTION_MODE)
            .AddServerCore()
            .AddCQRSServer();

        // TODO: Remove when common updated
        if (string.IsNullOrEmpty(AppSettings.REDIS.HOST))
        {
            services.AddDistributedMemoryCache();
        }
        else
        {
            services.AddRedis(AppSettings.REDIS, AppSettings.REDIS.INSTANCE_NAME + (AppSettings.PRODUCTION_MODE ? "_prod_" : "_dev_")); // TODO: Add this to common
        }

        // TODO: Add to common, so we have RANDOM, MEMORY etc
        if (AppSettings.DATABASE.CONNECTION_STRING == "MEMORY")
        {
            services.AddDbContextFactory<BerlinDbContext>(options =>
            {
                options
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors()
                    .UseInMemoryDatabase("BERLIN_IN_MEMORY_DB_CONTEXT")
                    .ConfigureWarnings((WarningsConfigurationBuilder _) => _.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            });
            services.AddScoped<IDbContext<BerlinDbContext>>(_ => _.GetRequiredService<BerlinDbContext>());
        }
        else
        {
            services
                .AddDatabase<BerlinDbContext>(
                    AppSettings.PRODUCTION_MODE,
                    AppSettings.DATABASE,
                    typeof(SqliteContextFactory));
        }
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app
            .UseHttpsRedirection()
            .UseRouting()
            .UseCors()
            .UseMiddleware<RequestResponseLoggingMiddleware>()
            .UseAuthentication()
            .UseAuthorization()
            .UseEndpoints(endpoints =>
            {
                endpoints
                    .MapBackendRemoteAuthenticationEndpoints<BerlinDbContext>()
                    .MapCQRSEndpoints()
                    .MapCommonHealthChecks();
            });
    }
}
