namespace mark.davison.berlin.api.test.Framework;

public class BerlinFinanceHttpRepository : HttpRepository
{
    public BerlinFinanceHttpRepository(string baseUri, JsonSerializerOptions options) : base(baseUri, new HttpClient(), options)
    {

    }
    public BerlinFinanceHttpRepository(string baseUri, HttpClient client, JsonSerializerOptions options) : base(baseUri, client, options)
    {

    }
}

public class BerlinApiWebApplicationFactory : WebApplicationFactory<Startup>, ICommonWebApplicationFactory<AppSettings>
{
    public IServiceProvider ServiceProvider => base.Services;

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

        //services.UseCQRSServer();
        //services.AddCommandCQRS();
        services.AddScoped<ICurrentUserContext, CurrentUserContext>(_ =>
        {
            var context = new CurrentUserContext();
            if (ModifyCurrentUserContext != null) { ModifyCurrentUserContext(_, context); }
            return context;
        });
    }

    public Action<IServiceProvider, CurrentUserContext>? ModifyCurrentUserContext { get; set; }
}
