﻿using Microsoft.JSInterop;

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
                var authConfig = _.GetRequiredService<IAuthenticationConfig>();
                if (authConfig.BffBase == WebConstants.LocalBffRoot)
                {
                    authConfig.SetBffBase(string.Empty);
                }
                if (string.IsNullOrEmpty(authConfig.BffBase))
                {
                    var jsRuntime = _.GetRequiredService<IJSRuntime>();

                    if (jsRuntime is IJSInProcessRuntime jsInProcessRuntime)
                    {

                        string bffRoot = jsInProcessRuntime.Invoke<string>("GetBffUri", null);

                        if (string.IsNullOrEmpty(bffRoot))
                        {
                            bffRoot = WebConstants.LocalBffRoot;
                        }

                        authConfig.SetBffBase(bffRoot);
                    }
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
