namespace mark.davison.shared.server.services.Igntion;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection UseSharedServerServices(this IServiceCollection services)
    {
        services
            .AddScoped<IFandomService, FandomService>()
            .AddScoped<IAuthorService, AuthorService>()
            .AddTransient<INotificationCreationService, NotificationCreationService>();
        return services;
    }
}
