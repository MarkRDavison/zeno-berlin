namespace mark.davison.berlin.web.features.Store.ManageStoryUseCase;

[Effect]
public sealed class ManageStoryEffects
{
    private readonly IClientHttpRepository _repository;

    public ManageStoryEffects(IClientHttpRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleFetchManageStoryActionAsync(FetchManageStoryAction action, IDispatcher dispatcher)
    {
        var queryRequest = new ManageStoryQueryRequest { StoryId = action.StoryId };

        var queryResponse = await _repository.Get<ManageStoryQueryRequest, ManageStoryQueryResponse>(queryRequest, CancellationToken.None);

        var actionResponse = new FetchManageStoryActionResponse
        {
            ActionId = action.ActionId,
            Errors = [.. queryResponse.Errors],
            Warnings = [.. queryResponse.Warnings],
            Value = queryResponse.Value
        };

        dispatcher.Dispatch(actionResponse);
    }

    public Task HandleUpdateManageStoryActionResponseAsync(UpdateManageStoryActionResponse response, IDispatcher dispatcher)
    {
        var action = new FetchManageStoryAction
        {
            ActionId = response.ActionId,
            StoryId = response.StoryId,
            SetLoading = false
        };

        dispatcher.Dispatch(action);

        return Task.CompletedTask;
    }

    public async Task HandleAddManageStoryUpdateActionAsync(AddManageStoryUpdateAction action, IDispatcher dispatcher)
    {
        var commandRequest = new AddStoryUpdateCommandRequest
        {
            StoryId = action.StoryId,
            CurrentChapters = action.CurrentChapters,
            TotalChapters = action.TotalChapters,
            Complete = action.Complete,
            UpdateDate = action.UpdateDate
        };

        var commandResponse = await _repository.Post<AddStoryUpdateCommandRequest, AddStoryUpdateCommandResponse>(commandRequest, CancellationToken.None);

        var response = new AddManageStoryUpdateActionResponse
        {
            ActionId = action.ActionId,
            Errors = [.. commandResponse.Errors],
            Warnings = [.. commandResponse.Warnings],
            Value = commandResponse.Value
        };

        dispatcher.Dispatch(response);
    }
}
