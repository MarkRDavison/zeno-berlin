namespace mark.davison.berlin.web.features.Store.DashboardListUseCase;

[Effect]
public sealed class DashboardListEffects
{
    private readonly IClientHttpRepository _repository;

    public DashboardListEffects(IClientHttpRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleFetchDashboardListActionAsync(FetchDashboardListAction action, IDispatcher dispatcher)
    {
        var queryRequest = new DashboardListQueryRequest { Maximum = action.Maximum };

        var queryResponse = await _repository.Get<DashboardListQueryRequest, DashboardListQueryResponse>(queryRequest, CancellationToken.None);

        var actionResponse = new FetchDashboardListActionResponse
        {
            ActionId = action.ActionId,
            Errors = [.. queryResponse.Errors],
            Warnings = [.. queryResponse.Warnings],
            Value = queryResponse.Value
        };

        dispatcher.Dispatch(actionResponse);
    }
}
