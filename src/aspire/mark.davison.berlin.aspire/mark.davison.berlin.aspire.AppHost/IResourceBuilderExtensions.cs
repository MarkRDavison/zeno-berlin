namespace mark.davison.berlin.aspire.ServiceDefaults;

public static class IResourceBuilderExtensions
{
    private const string _httpsEndpointName = "https";

    public static IResourceBuilder<TProject> WithCommonHealthChecks<TProject>(this IResourceBuilder<TProject> builder)
        where TProject : IResourceWithEndpoints
    {
        return builder
            .WithHttpHealthCheck("/health/startup")
            .WithHttpHealthCheck("/health/readiness")
            .WithHttpHealthCheck("/health/liveness");
    }

    public static IResourceBuilder<TProject> WithNonProxiedHttpsEndpoint<TProject>(this IResourceBuilder<TProject> builder)
        where TProject : IResourceWithEndpoints
    {
        return builder.WithEndpoint(_httpsEndpointName, endpoint => endpoint.IsProxied = false);
    }
}
