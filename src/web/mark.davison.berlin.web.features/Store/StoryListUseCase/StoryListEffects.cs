namespace mark.davison.berlin.web.features.Store.StoryListUseCase;

public class StoryListEffects
{
    private readonly IClientHttpRepository _repository;

    public StoryListEffects(IClientHttpRepository repository)
    {
        _repository = repository;
    }

    [EffectMethod]
    public async Task HandleAddStoryListActionAsync(AddStoryListAction action, IDispatcher dispatcher)
    {
        var commandRequest = new AddStoryCommandRequest
        {
            StoryAddress = action.StoryAddress
        };

        var commandResponse = await _repository.Post<AddStoryCommandResponse, AddStoryCommandRequest>(commandRequest, CancellationToken.None);

        // TODO: Create response from response, copies Errors/Warnings in common
        var actionResponse = new AddStoryListActionResponse
        {
            Errors = [.. commandResponse.Errors],
            Warnings = [.. commandResponse.Warnings],
            Value = commandResponse.Value
        };

        dispatcher.Dispatch(actionResponse);
    }
}
