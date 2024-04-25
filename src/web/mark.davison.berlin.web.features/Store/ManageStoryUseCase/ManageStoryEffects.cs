namespace mark.davison.berlin.web.features.Store.ManageStoryUseCase;

public class ManageStoryEffects
{
    private readonly IClientHttpRepository _repository;

    public ManageStoryEffects(IClientHttpRepository repository)
    {
        _repository = repository;
    }

    [EffectMethod]
    public async Task HandleFetchManageStoryActionAsync(FetchManageStoryAction action, IDispatcher dispatcher)
    {
        var queryRequest = new ManageStoryQueryRequest { StoryId = action.StoryId };

        var queryResponse = await _repository.Get<ManageStoryQueryResponse, ManageStoryQueryRequest>(queryRequest, CancellationToken.None);

        var actionResponse = new FetchManageStoryActionResponse
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
