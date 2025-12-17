namespace mark.davison.berlin.api.jobs;

[UseCQRSServer]
public class Startup
{
    public JobsAppSettings AppSettings { get; set; } = default!;
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
            .AddScoped<ICurrentUserContext, JobCurrentUserContext>()
            .AddHealthCheckServices<ApplicationHealthStateHostedService>()
            .UseBerlinLogic(AppSettings.PRODUCTION_MODE)
            .UseSharedServerServices(
                !string.IsNullOrEmpty(AppSettings.REDIS.HOST),
                AppSettings.NOTIFICATIONS.MATRIX,
                AppSettings.NOTIFICATIONS.CONSOLE)
            .AddRedis(AppSettings.REDIS, AppSettings.REDIS.INSTANCE_NAME + (AppSettings.PRODUCTION_MODE ? "_prod_" : "_dev_"))
            .AddSingleton<IJobOrchestrationService, JobOrchestrationService>()
            .AddSingleton<ICheckJobsService, CheckJobsService>()
            .AddServerCore()
            .AddCQRSServer();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app
            .UseHttpsRedirection()
            .UseRouting()
            /*.Use(async (HttpContext ctx, RequestDelegate next) =>
            {
                ctx.User = await ctx.RequestServices
                    .GetRequiredService<ICurrentUserContext>()
                    .PopulateFromPrincipal(ctx.User, string.Empty);

                await next(ctx);
            })*/
            .UseEndpoints(_ =>
            {
                _.MapCommonHealthChecks();
            });
    }
}
