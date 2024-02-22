namespace mark.davison.berlin.api.Ignition;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection UseDataSeeders(this IServiceCollection services)
    {
        services.AddTransient<IBerlinDataSeeder, BerlinDataSeeder>();
        return services;
    }

    public static IServiceCollection UseCronJobs(this IServiceCollection services, AppSettings appSettings)
    {
        services.AddCronJob<UpdateStoriesCronJob>(_ =>
        {
            _.TimeZoneInfo = TimeZoneInfo.Local;
            _.CronExpression = appSettings.STORY_UPDATE_CRON;
        });
        return services;
    }
}
