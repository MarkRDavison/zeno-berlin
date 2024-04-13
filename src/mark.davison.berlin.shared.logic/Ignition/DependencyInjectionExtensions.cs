namespace mark.davison.berlin.shared.logic.Ignition;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection UseBerlinLogic(this IServiceCollection services)
    {
        services.AddHttpClient(nameof(Ao3StoryInfoProcessor));
        services.AddKeyedScoped<IStoryInfoProcessor>(
            SiteConstants.ArchiveOfOurOwn_ShortName,
            (_, __) => new Ao3StoryInfoProcessor(
                _.GetRequiredService<IHttpClientFactory>(),
                _.GetRequiredService<IRateLimitServiceFactory>(),
                _.GetRequiredService<IOptions<Ao3Config>>()));

        return services;
    }
}
