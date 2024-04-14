namespace mark.davison.shared.services.Ignition;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection UseSharedServices(this IServiceCollection services, NotificationSettings settings)
    {
        services.AddSingleton<IStoryNotificationHub, StoryNotificationHub>();
        services.AddTransient<IRateLimitServiceFactory, RateLimitServiceFactory>();
        services.UseNotifications(settings);
        return services;
    }

    public static IServiceCollection UseNotifications(this IServiceCollection services, NotificationSettings settings)
    {
        services.UseMatrixNotifications(settings.MATRIX);
        services.UseConsoleNotifications(settings.CONSOLE);
        return services;
    }

    public static IServiceCollection UseMatrixNotifications(this IServiceCollection services, MatrixNotificationSettings matrixSettings)
    {
        if (!matrixSettings.ENABLED)
        {
            return services;
        }

        services.AddHttpClient(MatrixConstants.HttpClientName);
        services.AddSingleton<IMatrixNotificationService, MatrixNotificationService>();

        return services;
    }

    public static IServiceCollection UseConsoleNotifications(this IServiceCollection services, ConsoleNotificationSettings consoleSettings)
    {
        if (!consoleSettings.ENABLED)
        {
            return services;
        }

        services.AddSingleton<IConsoleNotificationService, ConsoleNotificationService>();

        return services;
    }
}
