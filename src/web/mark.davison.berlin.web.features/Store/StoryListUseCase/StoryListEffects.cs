namespace mark.davison.berlin.web.features.Store.StoryListUseCase;

[Effect]
public sealed class StoryListEffects
{
    private readonly IClientHttpRepository _repository;

    public StoryListEffects(IClientHttpRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleFetchStoryListActionAsync(FetchStoryListAction action, IDispatcher dispatcher)
    {
        var queryRequest = new StoryListQueryRequest { }; // TODO: Rename/ restructure

        var queryResponse = await _repository.Get<StoryListQueryRequest, StoryListQueryResponse>(queryRequest, CancellationToken.None);

        var actionResponse = new FetchStoryListActionResponse
        {
            ActionId = action.ActionId,
            Errors = [.. queryResponse.Errors],
            Warnings = [.. queryResponse.Warnings],
            Value = queryResponse.Value ?? []
        };

        // TODO: Framework to dispatch general ***something went wrong***

        dispatcher.Dispatch(actionResponse);
    }
}
