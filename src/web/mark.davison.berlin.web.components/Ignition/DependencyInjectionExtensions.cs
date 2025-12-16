namespace mark.davison.berlin.web.components.Ignition;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection UseBerlinComponents(this IServiceCollection services)
    {
        services.UseClientRepository(WebConstants.ApiClientName, WebConstants.LocalBffRoot);
        services.UseAuthentication(WebConstants.ApiClientName);
        services.UseClientCQRS(typeof(Routes));
        services.UseCommonClient(typeof(Routes));

        return services;
    }
}