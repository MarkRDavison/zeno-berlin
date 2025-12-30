namespace mark.davison.berlin.shared.server.services.Ignition;

public static class DependencyInjectionExtensions
{

    public static IServiceCollection UseSharedServerServices(
        this IServiceCollection services,
        bool redisInUse,
        MatrixNotificationSettings matrixNotificationSettings,
        ConsoleNotificationSettings consoleNotificationSettings)
    {
        services
            .AddScoped<IFandomService, FandomService>()
            .AddScoped<IAuthorService, AuthorService>()
            .AddScoped<ISiteService, SiteService>()
            .AddTransient<INotificationCreationService, NotificationCreationService>();

        services.UseLockPubSub(redisInUse);

        services.AddScoped<INotificationHub, NotificationHub>();

        if (matrixNotificationSettings.ENABLED)
        {
            services
                .AddScoped<INotificationService, MatrixNotificationService>()
                .AddScoped<IMatrixClient, MatrixClient>()
                .AddHttpClient(MatrixConstants.HttpClientName);
        }

        if (consoleNotificationSettings.ENABLED)
        {
            services
                .AddScoped<INotificationService, ConsoleNotificationService>();
        }

        return services;
    }

    public static IServiceCollection UseLockPubSub(
        this IServiceCollection services,
        bool redisInUse)
    {

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