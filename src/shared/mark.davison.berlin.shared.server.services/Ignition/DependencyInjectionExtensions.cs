namespace mark.davison.berlin.shared.server.services.Ignition;

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

        // TODO: Only if enabled
        services
            .AddScoped<INotificationHub, NotificationHub>()
            .AddScoped<INotificationService, ConsoleNotificationService>()
            .AddScoped<INotificationService, MatrixNotificationService>()
            .AddScoped<IMatrixClient, MatrixClient>()
            .AddHttpClient(MatrixConstants.HttpClientName);

        return services;
    }
}