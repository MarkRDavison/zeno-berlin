namespace mark.davison.berlin.shared.logic.Ignition;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection UseBerlinLogic(this IServiceCollection services, bool productionMode)
    {
        services.AddHttpClient(nameof(Ao3StoryInfoProcessor));
        services.AddKeyedScoped<IStoryInfoProcessor>(
            SiteConstants.ArchiveOfOurOwn_ShortName,
            (_, __) => new Ao3StoryInfoProcessor(
                _.GetRequiredService<IHttpClientFactory>().CreateClient(nameof(Ao3StoryInfoProcessor)),
                _.GetRequiredService<IRateLimitServiceFactory>(),
                _.GetRequiredService<IOptions<Ao3Config>>()));

        if (!productionMode)
        {
            services.AddHttpClient("FAKE_" + nameof(Ao3StoryInfoProcessor));
            services.AddKeyedScoped<IStoryInfoProcessor>(
                SiteConstants.FakeArchiveOfOurOwn_ShortName,
                (_, __) => new Ao3StoryInfoProcessor(
                    _.GetRequiredService<IHttpClientFactory>().CreateClient("FAKE_" + nameof(Ao3StoryInfoProcessor)),
                    _.GetRequiredService<IRateLimitServiceFactory>(),
                    Options.Create(new Ao3Config
                    {
                        RATE_DELAY = 0,
                        SITE_ADDRESS = SiteConstants.FakeArchiveOfOurOwn_Address
                    })));
        }

        return services;
    }
}
