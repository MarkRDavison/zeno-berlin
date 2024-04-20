namespace mark.davison.berlin.web.ui;

public class BerlinClientHttpRepository : ClientHttpRepository
{
    public BerlinClientHttpRepository(
        string remoteEndpoint,
        IHttpClientFactory httpClientFactory,
        ILogger<BerlinClientHttpRepository> logger
    ) : base(
        remoteEndpoint,
        httpClientFactory.CreateClient(WebConstants.ApiClientName),
        logger)
    {
    }
}
