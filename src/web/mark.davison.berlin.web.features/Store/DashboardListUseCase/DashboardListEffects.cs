namespace mark.davison.berlin.web.features.Store.DashboardListUseCase;

public class DashboardListEffects
{
    private readonly IClientHttpRepository _repository;

    public DashboardListEffects(IClientHttpRepository repository)
    {
        _repository = repository;
    }

    [EffectMethod]
    public async Task HandleFetchDashboardListActionAsync(FetchDashboardListAction action, IDispatcher dispatcher)
    {
        var queryRequest = new DashboardListQueryRequest { Maximum = action.Maximum };

        var queryResponse = await _repository.Get<DashboardListQueryResponse, DashboardListQueryRequest>(queryRequest, CancellationToken.None);

        var actionResponse = new FetchDashboardListActionResponse
        {
            ActionId = action.ActionId,
            Errors = [.. queryResponse.Errors],
            Warnings = [.. queryResponse.Warnings],
            Value = queryResponse.Value
        };

        // TODO: Framework to dispatch general ***something went wrong***

        dispatcher.Dispatch(actionResponse);
    }
}
