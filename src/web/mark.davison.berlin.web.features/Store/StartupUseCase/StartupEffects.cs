namespace mark.davison.berlin.web.features.Store.StartupUseCase;

[Effect]
public sealed class StartupEffects
{
    private readonly IClientHttpRepository _clientHttpRepository;

    public StartupEffects(IClientHttpRepository clientHttpRepository)
    {
        _clientHttpRepository = clientHttpRepository;
    }

    public async Task HandleFetchStartupActionAsync(FetchStartupAction action, IDispatcher dispatcher)
    {
        var request = new StartupQueryRequest { };
        var response = await _clientHttpRepository.Get<StartupQueryRequest, StartupQueryResponse>(request, CancellationToken.None);

        dispatcher.Dispatch(new UpdateStartupActionResponse
        {
            ActionId = action.ActionId,
            Errors = response.Errors,
            Warnings = response.Warnings,
            Value = response.Value
        });
    }
}
