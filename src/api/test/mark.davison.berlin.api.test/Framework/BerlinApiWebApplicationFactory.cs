namespace mark.davison.berlin.api.test.Framework;

public class BerlinHttpRepository : HttpRepository
{
    public BerlinHttpRepository(string baseUri, JsonSerializerOptions options) : base(baseUri, new HttpClient(), options)
    {

    }
    public BerlinHttpRepository(string baseUri, HttpClient client, JsonSerializerOptions options) : base(baseUri, client, options)
    {

    }
}

public class BerlinApiWebApplicationFactory : WebApplicationFactory<Startup>, ICommonWebApplicationFactory<AppSettings>
{
    public IServiceProvider ServiceProvider => base.Services;
    private readonly Dictionary<string, TestHttpMessageHandler> _messageHandlers = new();

    public BerlinApiWebApplicationFactory()
    {
        _messageHandlers.Add(nameof(Ao3StoryInfoProcessor), new TestHttpMessageHandler());
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, conf) => conf
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.integration.json", false));
        builder.ConfigureTestServices(ConfigureServices);
        builder.ConfigureLogging((WebHostBuilderContext context, ILoggingBuilder loggingBuilder) =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddConsole();
        });
    }

    protected virtual void ConfigureServices(IServiceCollection services)
    {
        //services.AddTransient<IFinanceDataSeeder, FinanceDataSeeder>(_ => // TODO: Remove lambda???
        //    new FinanceDataSeeder(
        //        _.GetRequiredService<IServiceScopeFactory>(),
        //        _.GetRequiredService<IOptions<AppSettings>>()
        //    ));
        //services.UseDataSeeders();


        services
            .AddHttpClient()
            .AddHttpContextAccessor();

        services.AddScoped<ICurrentUserContext, CurrentUserContext>(_ =>
        {
            var context = new CurrentUserContext();
            if (ModifyCurrentUserContext != null) { ModifyCurrentUserContext(_, context); }
            return context;
        });

        foreach (var (name, handler) in _messageHandlers)
        {
            services
                .AddHttpClient(name)
                .ConfigurePrimaryHttpMessageHandler(_ => handler);
        }

    }

    public TestHttpMessageHandler GetMessageHandler(string httpClientName) => _messageHandlers[httpClientName];

    public Action<IServiceProvider, CurrentUserContext>? ModifyCurrentUserContext { get; set; }
}
