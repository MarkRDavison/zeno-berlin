namespace mark.davison.berlin.api.Ignition;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection UseDataSeeders(this IServiceCollection services)
    {
        services.AddTransient<IBerlinDataSeeder, BerlinDataSeeder>();
        return services;
    }
}
