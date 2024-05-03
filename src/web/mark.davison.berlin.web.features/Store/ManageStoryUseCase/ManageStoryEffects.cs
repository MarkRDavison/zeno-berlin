using mark.davison.berlin.shared.models.dtos.Scenarios.Commands.AddStoryUpdate;

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

    [EffectMethod]
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

    [EffectMethod]
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

        var commandResponse = await _repository.Post<AddStoryUpdateCommandResponse, AddStoryUpdateCommandRequest>(commandRequest, CancellationToken.None);

        // TODO: Framework to dispatch general ***something went wrong***

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
