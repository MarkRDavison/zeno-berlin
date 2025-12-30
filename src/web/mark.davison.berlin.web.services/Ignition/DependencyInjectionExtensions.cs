namespace mark.davison.berlin.web.services.Ignition;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection UseBerlinServices(this IServiceCollection services)
    {
        services
            .AddSingleton<IDateService>(_ => new DateService(DateService.DateMode.Local))
            .AddSingleton<IClientJobHttpRepository, ClientJobHttpRepository>();

        return services;
    }
}
