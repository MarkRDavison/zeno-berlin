namespace mark.davison.berlin.web.features.Store.PotentialStoryUseCase;

[Effect]
public sealed class PotentialStoryEffects
{
    private readonly IClientHttpRepository _repository;

    public PotentialStoryEffects(IClientHttpRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleFetchPotentialStoriesAsync(FetchPotentialStoriesAction action, IDispatcher dispatcher)
    {
        var queryRequest = new PotentialStoryListQueryRequest();

        var queryResponse = await _repository.Get<PotentialStoryListQueryRequest, PotentialStoryListQueryResponse>(queryRequest, CancellationToken.None);

        var actionResponse = new FetchPotentialStoriesActionResponse
        {
            ActionId = action.ActionId,
            Errors = [.. queryResponse.Errors],
            Warnings = [.. queryResponse.Warnings],
            Value = queryResponse.Value
        };

        dispatcher.Dispatch(actionResponse);
    }

    public async Task HandleAddPotentialStoryAsync(AddPotentialStoryAction action, IDispatcher dispatcher)
    {
        var commandRequest = new AddPotentialStoryCommandRequest { StoryAddress = action.StoryAddress };

        var commandResponse = await _repository.Post<AddPotentialStoryCommandRequest, AddPotentialStoryCommandResponse>(commandRequest, CancellationToken.None);

        var actionResponse = new AddPotentialStoryActionResponse
        {
            ActionId = action.ActionId,
            Errors = [.. commandResponse.Errors],
            Warnings = [.. commandResponse.Warnings],
            Value = commandResponse.Value
        };

        dispatcher.Dispatch(actionResponse);
    }

    public async Task HandleDeletePotentialStoryAsync(DeletePotentialStoryAction action, IDispatcher dispatcher)
    {
        var commandRequest = new DeletePotentialStoryCommandRequest { Id = action.Id };

        var commandResponse = await _repository.Post<DeletePotentialStoryCommandRequest, DeletePotentialStoryCommandResponse>(commandRequest, CancellationToken.None);

        var actionResponse = new DeletePotentialStoryActionResponse
        {
            ActionId = action.ActionId,
            Errors = [.. commandResponse.Errors],
            Warnings = [.. commandResponse.Warnings],
            Value = action.Id
        };

        dispatcher.Dispatch(actionResponse);
    }

    public async Task HandleGrabPotentialStoryAsync(GrabPotentialStoryAction action, IDispatcher dispatcher)
    {
        var commandRequest = new GrabPotentialStoryCommandRequest { Id = action.Id };

        var commandResponse = await _repository.Post<GrabPotentialStoryCommandRequest, GrabPotentialStoryCommandResponse>(commandRequest, CancellationToken.None);

        var actionResponse = new GrabPotentialStoryActionResponse
        {
            ActionId = action.ActionId,
            Errors = [.. commandResponse.Errors],
            Warnings = [.. commandResponse.Warnings],
            Value = commandResponse.Value
        };

        dispatcher.Dispatch(actionResponse);
    }
}
