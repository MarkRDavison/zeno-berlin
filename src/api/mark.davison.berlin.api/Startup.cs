namespace mark.davison.berlin.api;

[UseCQRSServer]
public class Startup(IConfiguration Configuration)
{
    public ApiAppSettings AppSettings { get; set; } = new();

    public void ConfigureServices(IServiceCollection services)
    {
        AppSettings = services.BindAppSettings(Configuration);
        AppSettings.DATABASE.CONNECTION_STRING = "RANDOM";

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
            .AddRedis(AppSettings.REDIS, AppSettings.REDIS.INSTANCE_NAME + (AppSettings.PRODUCTION_MODE ? "_prod_" : "_dev_")) // TODO: Add this to common
            .AddDatabase<BerlinDbContext>(
                AppSettings.PRODUCTION_MODE,
                AppSettings.DATABASE,
                typeof(SqliteContextFactory))
            .AddCoreDbContext<BerlinDbContext>()
            .AddHealthCheckServices<ApplicationHealthStateHostedService>()
            .UseSharedServerServices(!string.IsNullOrEmpty(AppSettings.REDIS.HOST))
            .AddServerCore()
            .AddCQRSServer();
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
