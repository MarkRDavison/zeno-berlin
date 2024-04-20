namespace mark.davison.berlin.bff.web;

public class Startup
{
    const string AuthorityToWellKnown = "/.well-known/openid-configuration";

    public IConfiguration Configuration { get; }
    public AppSettings AppSettings { get; set; } = null!;

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        var AppSettings = services.ConfigureSettingsServices<AppSettings>(Configuration);
        if (AppSettings == null) { throw new InvalidOperationException(); }

        services
            .ConfigureHealthCheckServices();

        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                builder => builder
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .SetIsOriginAllowed((host) => true) // TODO: Config driven
                    .AllowAnyHeader());
        });

        services.AddAuthentication(ZenoAuthenticationConstants.ZenoAuthenticationScheme);
        services.AddAuthorization();
        services.AddTransient<ICustomZenoAuthenticationActions, BerlinCustomZenoAuthenticationActions>();
        services.AddHttpClient("PROXY");

        services.UseRedisSession(
            AppSettings.AUTH,
            AppSettings.REDIS,
            AppSettings.SECTION.ToLower(),
            AppSettings.PRODUCTION_MODE);

        services.AddZenoAuthentication(_ =>
        {
            if (string.IsNullOrEmpty(AppSettings.AUTH.AUTHORITY))
            {
                throw new InvalidOperationException();
            }
            _.Scope = AppSettings.AUTH.SCOPE;
            _.WebOrigin = AppSettings.WEB_ORIGIN;
            _.BffOrigin = AppSettings.BFF_ORIGIN;
            _.ClientId = AppSettings.AUTH.CLIENT_ID;
            _.ClientSecret = AppSettings.AUTH.CLIENT_SECRET;
            _.OpenIdConnectWellKnownUri = AppSettings.AUTH.AUTHORITY + AuthorityToWellKnown;
        });

        services.AddScoped<ICurrentUserContext, CurrentUserContext>();
        services.AddSingleton<IDateService>(new DateService(DateService.DateMode.Utc));

        services.AddSingleton<IHttpRepository>(_ =>
        {
            var options = SerializationHelpers.CreateStandardSerializationOptions();
            return new BerlinHttpRepository(AppSettings.API_ORIGIN, options);
        });

        services
            .AddHttpClient()
            .AddHttpContextAccessor();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSession();

        app.UseMiddleware<RequestResponseLoggingMiddleware>();

        app.UseCors("CorsPolicy");

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseMiddleware<HydrateAuthenticationFromSessionMiddleware>();

        app.UseEndpoints(endpoints =>
        {
            endpoints
                .MapHealthChecks();

            endpoints
                .UseAuthenticationEndpoints();

            MapProxyCQRSGet(endpoints, "/api/startup-query");
            MapProxyCQRSGet(endpoints, "/api/story-list-query");
            MapProxyCQRSPost(endpoints, "/api/add-story-command");
            MapProxyCQRSPost(endpoints, "/api/edit-story-command");
            MapProxyCQRSPost(endpoints, "/api/delete-story-command");
            MapProxyCQRSPost(endpoints, "/api/update-stories-command");
        });
    }

    static void MapProxyCQRSPost(IEndpointRouteBuilder endpoints, string path)
    {
        endpoints.MapPost(
            path,
            async (HttpContext context, IOptions<AppSettings> options, IHttpClientFactory httpClientFactory, ICurrentUserContext currentUserContext, CancellationToken cancellationToken) =>
            {
                if (string.IsNullOrEmpty(currentUserContext.Token))
                {
                    return Results.Unauthorized();
                }
                var client = httpClientFactory.CreateClient("PROXY");

                var headers = HeaderParameters.Auth(currentUserContext.Token, currentUserContext.CurrentUser);

                var request = new HttpRequestMessage(HttpMethod.Post, $"{options.Value.API_ORIGIN.TrimEnd('/')}{path}");

                foreach (var k in headers)
                {
                    request.Headers.Add(k.Key, k.Value);
                }

                request.Content = new StreamContent(context.Request.Body);

                var response = await client.SendAsync(request, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return Results.Text(content);
                }

                return Results.BadRequest(new Response
                {
                    Errors = new() { $"Error: {response.StatusCode}" }
                });
            });
    }

    static void MapProxyCQRSGet(IEndpointRouteBuilder endpoints, string path)
    {
        endpoints.MapGet(
            path,
            async (HttpContext context, IOptions<AppSettings> options, IHttpClientFactory httpClientFactory, ICurrentUserContext currentUserContext, CancellationToken cancellationToken) =>
            {
                if (string.IsNullOrEmpty(currentUserContext.Token))
                {
                    return Results.Unauthorized();
                }

                var client = httpClientFactory.CreateClient("PROXY");

                var headers = HeaderParameters.Auth(currentUserContext.Token, currentUserContext.CurrentUser);

                var request = new HttpRequestMessage(HttpMethod.Get, $"{options.Value.API_ORIGIN.TrimEnd('/')}{path}{context.Request.QueryString}");

                foreach (var k in headers)
                {
                    request.Headers.Add(k.Key, k.Value);
                }

                var response = await client.SendAsync(request, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return Results.Text(content);
                }

                return Results.BadRequest(new Response
                {
                    Errors = new() { $"Error: {response.StatusCode}" }
                });
            });
    }
}
