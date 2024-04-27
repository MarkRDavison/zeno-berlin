using Microsoft.JSInterop;

namespace mark.davison.berlin.web.ui.Ignition;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection UseBerlinWeb(this IServiceCollection services, IAuthenticationConfig authConfig)
    {

        services
            .UseBerlinComponents()
            .AddSingleton<IAuthenticationConfig>(authConfig)
            .AddSingleton<IAuthenticationContext, AuthenticationContext>()
            .AddSingleton<IDateService>(_ => new DateService(DateService.DateMode.Local))
            .AddScoped<IStoreHelper, StoreHelper>()
            .AddFluxor(_ => _.ScanAssemblies(typeof(Program).Assembly, typeof(FeaturesRootType).Assembly))
            .AddSingleton<IClientNavigationManager, ClientNavigationManager>()
            .AddSingleton<IClientHttpRepository>(_ =>
            {
                Console.WriteLine("Attempting to read bff value");
                var authConfig = _.GetRequiredService<IAuthenticationConfig>();
                Console.WriteLine("Value is: {0}", authConfig.BffBase);

                if (string.IsNullOrEmpty(authConfig.BffBase))
                {
                    var jsRuntime = _.GetRequiredService<IJSRuntime>();
                    var bffRootTask = jsRuntime.InvokeAsync<string>("GetBffUri", null);

                    var bffRoot = bffRootTask.GetAwaiter().GetResult();

                    authConfig.SetBffBase(bffRoot);
                    Console.WriteLine("bffRoot was null, setting it to {0}", bffRoot);
                }

                return new BerlinClientHttpRepository(
                        _.GetRequiredService<IAuthenticationConfig>().BffBase,
                        _.GetRequiredService<IHttpClientFactory>(),
                        _.GetRequiredService<ILogger<BerlinClientHttpRepository>>());
            })
            .UseClientCQRS(typeof(Program), typeof(FeaturesRootType))
            .AddHttpClient(WebConstants.ApiClientName)
            .AddHttpMessageHandler(_ => new CookieHandler());

        return services;
    }
}
