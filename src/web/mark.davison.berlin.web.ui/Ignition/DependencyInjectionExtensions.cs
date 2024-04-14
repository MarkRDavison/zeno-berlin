namespace mark.davison.berlin.web.ui.Ignition;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection UseBerlinWeb(this IServiceCollection services, IAuthenticationConfig authConfig)
    {

        services
            .AddSingleton<IAuthenticationConfig>(authConfig)
            .AddSingleton<IAuthenticationContext, AuthenticationContext>()
            .AddFluxor(_ => _.ScanAssemblies(typeof(Program).Assembly, typeof(FeaturesRootType).Assembly))
            .AddSingleton<IClientNavigationManager, ClientNavigationManager>()
            .AddSingleton<IClientHttpRepository>(_ => new BerlinClientHttpRepository(
                        _.GetRequiredService<IAuthenticationConfig>().BffBase,
                        _.GetRequiredService<IHttpClientFactory>()))
            .UseCQRS(typeof(Program), typeof(FeaturesRootType))
            .AddHttpClient(WebConstants.ApiClientName)
            .AddHttpMessageHandler(_ => new CookieHandler());
        return services;
    }
}
