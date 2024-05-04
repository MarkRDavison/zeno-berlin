namespace mark.davison.berlin.api.Ignition;

public static class DependencyInjectionExtensions
{
    private const string MonthlyCronExpression = "0 3 1,15 * *"; // 25 days is int.MaxValue, so basically if its the middle of the month skip it

    public static IServiceCollection UseDataSeeders(this IServiceCollection services)
    {
        services.AddTransient<IBerlinDataSeeder, BerlinDataSeeder>();
        return services;
    }

    public static IServiceCollection UseCronJobs(this IServiceCollection services, AppSettings appSettings)
    {
        services
            .AddCronJob<UpdateStoriesCronJob>(_ =>
            {
                _.TimeZoneInfo = TimeZoneInfo.Local;
                _.CronExpression = appSettings.STORY_UPDATE_CRON;
            })
            .AddCronJob<MonthlyNotificationsCronJob>(_ =>
            {
                _.TimeZoneInfo = TimeZoneInfo.Local;
                _.CronExpression = MonthlyCronExpression;
            });

        return services;
    }
}
