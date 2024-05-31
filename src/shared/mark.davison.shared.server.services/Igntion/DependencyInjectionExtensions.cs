namespace mark.davison.shared.server.services.Igntion;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection UseSharedServerServices(this IServiceCollection services, bool redisInUse)
    {
        services
            .AddScoped<IFandomService, FandomService>()
            .AddScoped<IAuthorService, AuthorService>()
            .AddScoped<ISiteService, SiteService>()
            .AddTransient<INotificationCreationService, NotificationCreationService>();

        if (redisInUse)
        {
            services
                .AddSingleton<IDistributedPubSub, DistributedPubSub>()
                .AddSingleton<ILockService, LockService>()
                .AddSingleton<IRedisService, RedisService>();
        }
        else
        {
            services
                .AddSingleton<IDistributedPubSub, InMemoryDisutributedPubSub>()
                .AddSingleton<ILockService, InMemoryLockService>();
        }

        return services;
    }
}
