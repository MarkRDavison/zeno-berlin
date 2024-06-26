﻿namespace mark.davison.berlin.web.components.Ignition;

public static class DependecyInjectionExtensions
{
    public static IServiceCollection UseBerlinComponents(
        this IServiceCollection services,
        IAuthenticationConfig authConfig)
    {
        services.UseCommonClient(authConfig, typeof(Routes));
        services.AddSingleton<IClientJobHttpRepository, ClientJobHttpRepository>();
        return services;
    }
}