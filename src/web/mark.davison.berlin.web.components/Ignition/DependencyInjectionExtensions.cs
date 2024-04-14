namespace mark.davison.berlin.web.components.Ignition;

public static class DependecyInjectionExtensions
{
    public static IServiceCollection UseBerlinComponents(this IServiceCollection services)
    {
        services.AddMudServices();

        return services;
    }
}