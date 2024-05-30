namespace mark.davison.berlin.bff.web;

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
            .UseCookieOidcAuth(
                AppSettings.AUTH,
                AppSettings.CLAIMS, _ =>
                {
                    if (!AppSettings.PRODUCTION_MODE)
                    {
                        _.OnUserCreated = async (IServiceProvider services, User user, string access_token) =>
                        {
                            await Task.CompletedTask;
                        };
                    }
                },
                AppSettings.API_ORIGIN)
            .AddHealthCheckServices()
            .AddAuthorization()
            .AddEndpointsApiExplorer()
            .AddSwaggerGen()
            .AddScoped<ICurrentUserContext, CurrentUserContext>()
            .AddSingleton<IDateService>(new DateService(DateService.DateMode.Utc));

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

        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app
            .UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedFor
            })
            .UseMiddleware<RequestResponseLoggingMiddleware>()
            .UseRouting()
            .UseAuthentication()
            .UseMiddleware<CheckAccessTokenValidityMiddleware>()
            .UseAuthorization()
            .UseEndpoints(endpoints =>
            {
                endpoints
                    .UseApiProxy(AppSettings.API_ORIGIN)
                    .UseAuthEndpoints(AppSettings.WEB_ORIGIN)
                    .MapHealthChecks();
            });
    }

}
