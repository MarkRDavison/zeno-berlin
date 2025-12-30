namespace mark.davison.berlin.shared.logic.Ignition;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection UseBerlinLogic(this IServiceCollection services, bool productionMode)
    {
        services.AddHttpClient(nameof(Ao3StoryInfoProcessor));
        services
            .AddKeyedSingleton<IRateLimitService>(SiteConstants.ArchiveOfOurOwn_ShortName, (_, __) =>
            {
                var config = _.GetRequiredService<IOptions<Ao3Config>>();
                return new RateLimitService(1, TimeSpan.FromSeconds(config.Value.RATE_DELAY), 100);
            })
            .AddKeyedScoped<IStoryInfoProcessor>(
                SiteConstants.ArchiveOfOurOwn_ShortName,
                (_, __) => new Ao3StoryInfoProcessor(
                    _.GetRequiredService<IHttpClientFactory>().CreateClient(nameof(Ao3StoryInfoProcessor)),
                    _.GetRequiredKeyedService<IRateLimitService>(SiteConstants.ArchiveOfOurOwn_ShortName),
                    _.GetRequiredService<IOptions<Ao3Config>>(),
                    _.GetRequiredService<ILogger<Ao3StoryInfoProcessor>>()));

        if (!productionMode)
        {
            services.AddHttpClient("FAKE_" + nameof(Ao3StoryInfoProcessor));
            services
                .AddKeyedSingleton<IRateLimitService>(SiteConstants.FakeArchiveOfOurOwn_ShortName, (_, __) =>
                {
                    return new RateLimitService(1, TimeSpan.Zero, int.MaxValue);
                })
                .AddKeyedScoped<IStoryInfoProcessor>(
                    SiteConstants.FakeArchiveOfOurOwn_ShortName,
                    (_, __) => new Ao3StoryInfoProcessor(
                        _.GetRequiredService<IHttpClientFactory>().CreateClient("FAKE_" + nameof(Ao3StoryInfoProcessor)),
                        _.GetRequiredKeyedService<IRateLimitService>(SiteConstants.FakeArchiveOfOurOwn_ShortName),
                        Options.Create(new Ao3Config
                        {
                            RATE_DELAY = 0,
                            SITE_ADDRESS = SiteConstants.FakeArchiveOfOurOwn_Address
                        }),
                        _.GetRequiredService<ILogger<Ao3StoryInfoProcessor>>()));
        }

        return services;
    }
}
