namespace mark.davison.berlin.web.features.Ignition;

[UseState]
public static class DependencyInjectionExtensions
{
    public static IServiceCollection UseBerlinFeatures(this IServiceCollection services)
    {
        services.AddClientState();
        return services;
    }
}