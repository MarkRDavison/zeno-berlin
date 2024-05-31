namespace mark.davison.berlin.web.features.Store.PotentialStoryUseCase;

public sealed class PotentialStoryEffects
{
    private readonly IClientHttpRepository _repository;

    public PotentialStoryEffects(IClientHttpRepository repository)
    {
        _repository = repository;
    }

    [EffectMethod]
    public async Task HandleFetchPotentialStoriesAsync(FetchPotentialStoriesAction action, IDispatcher dispatcher)
    {
        var queryRequest = new PotentialStoryListQueryRequest();

        var queryResponse = await _repository.Get<PotentialStoryListQueryResponse, PotentialStoryListQueryRequest>(queryRequest, CancellationToken.None);

        var actionResponse = new FetchPotentialStoriesActionResponse
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
    public async Task HandleAddPotentialStoryAsync(AddPotentialStoryAction action, IDispatcher dispatcher)
    {
        var commandRequest = new AddPotentialStoryCommandRequest { StoryAddress = action.StoryAddress };

        var commandResponse = await _repository.Post<AddPotentialStoryCommandResponse, AddPotentialStoryCommandRequest>(commandRequest, CancellationToken.None);

        var actionResponse = new AddPotentialStoryActionResponse
        {
            ActionId = action.ActionId,
            Errors = [.. commandResponse.Errors],
            Warnings = [.. commandResponse.Warnings],
            Value = commandResponse.Value
        };

        // TODO: Framework to dispatch general ***something went wrong***

        dispatcher.Dispatch(actionResponse);
    }

    [EffectMethod]
    public async Task HandleDeletePotentialStoryAsync(DeletePotentialStoryAction action, IDispatcher dispatcher)
    {
        var commandRequest = new DeletePotentialStoryCommandRequest { Id = action.Id };

        var commandResponse = await _repository.Post<DeletePotentialStoryCommandResponse, DeletePotentialStoryCommandRequest>(commandRequest, CancellationToken.None);

        var actionResponse = new DeletePotentialStoryActionResponse
        {
            ActionId = action.ActionId,
            Errors = [.. commandResponse.Errors],
            Warnings = [.. commandResponse.Warnings],
            Value = action.Id
        };

        // TODO: Framework to dispatch general ***something went wrong***

        dispatcher.Dispatch(actionResponse);
    }

    [EffectMethod]
    public async Task HandleGrabPotentialStoryAsync(GrabPotentialStoryAction action, IDispatcher dispatcher)
    {
        var commandRequest = new GrabPotentialStoryCommandRequest { Id = action.Id };

        var commandResponse = await _repository.Post<GrabPotentialStoryCommandResponse, GrabPotentialStoryCommandRequest>(commandRequest, CancellationToken.None);

        var actionResponse = new GrabPotentialStoryActionResponse
        {
            ActionId = action.ActionId,
            Errors = [.. commandResponse.Errors],
            Warnings = [.. commandResponse.Warnings],
            Value = commandResponse.Value
        };

        // TODO: Framework to dispatch general ***something went wrong***

        dispatcher.Dispatch(actionResponse);
    }
}
