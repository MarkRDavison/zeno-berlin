namespace mark.davison.shared.services.Ignition;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection UseSharedServices(this IServiceCollection services)
    {
        services.AddSingleton<IStoryNotificationHub, StoryNotificationHub>();
        services.AddTransient<IRateLimitServiceFactory, RateLimitServiceFactory>();
        return services;
    }
}
