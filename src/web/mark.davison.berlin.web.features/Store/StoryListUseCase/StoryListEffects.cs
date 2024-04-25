namespace mark.davison.berlin.web.features.Store.StoryListUseCase;

public class StoryListEffects
{
    private readonly IClientHttpRepository _repository;

    public StoryListEffects(IClientHttpRepository repository)
    {
        _repository = repository;
    }

    [EffectMethod]
    public async Task HandleDeleteStoryListActionAsync(DeleteStoryListAction action, IDispatcher dispatcher)
    {
        var commandRequest = new DeleteStoryCommandRequest { StoryId = action.StoryId };

        var commandResponse = await _repository.Post<DeleteStoryCommandResponse, DeleteStoryCommandRequest>(commandRequest, CancellationToken.None);

        var actionResponse = new DeleteStoryListActionResponse
        {
            ActionId = action.ActionId,
            Errors = [.. commandResponse.Errors],
            Warnings = [.. commandResponse.Warnings],
            Value = commandResponse // TODO: Response<Response> seems gross
        };

        // TODO: Framework to dispatch general ***something went wrong***

        dispatcher.Dispatch(actionResponse);
    }

    [EffectMethod]
    public async Task HandleFetchStoryListActionAsync(FetchStoryListAction action, IDispatcher dispatcher)
    {
        var queryRequest = new StoryListQueryRequest { }; // TODO: Rename/ restructure

        var queryResponse = await _repository.Get<StoryListQueryResponse, StoryListQueryRequest>(queryRequest, CancellationToken.None);

        var actionResponse = new FetchStoryListActionResponse
        {
            ActionId = action.ActionId,
            Errors = [.. queryResponse.Errors],
            Warnings = [.. queryResponse.Warnings],
            Value = queryResponse.Value?.Stories ?? []
        };

        // TODO: Framework to dispatch general ***something went wrong***

        dispatcher.Dispatch(actionResponse);
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
            ActionId = action.ActionId,
            Errors = [.. commandResponse.Errors],
            Warnings = [.. commandResponse.Warnings],
            Value = commandResponse.Value
        };

        // TODO: Framework to dispatch general ***something went wrong***

        dispatcher.Dispatch(actionResponse);
    }

    [EffectMethod]
    public async Task HandleSetFavouriteStoryListActionAsync(SetFavouriteStoryListAction action, IDispatcher dispatcher)
    {
        var commandRequest = new EditStoryCommandRequest
        {
            StoryId = action.StoryId,
            Changes =
            [
                new DiscriminatedPropertyChangeset
                {
                    Name = nameof(StoryDto.Favourite),
                    PropertyType = typeof(bool).FullName!,
                    Value = action.IsFavourite
                }
            ]
        };

        var commandResponse = await _repository.Post<EditStoryCommandResponse, EditStoryCommandRequest>(commandRequest, CancellationToken.None);

        if (!commandResponse.SuccessWithValue)
        {
            // TODO: Re-query it????
            return;
        }

        // TODO: Create response from response, copies Errors/Warnings in common
        var actionResponse = new SetFavouriteStoryListActionResponse
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
    public async Task HandleUpdateStoryListActionAsync(UpdateStoryListAction action, IDispatcher dispatcher)
    {
        // TODO: Naming -> UpdateStoriesCommandRequest
        var commandRequest = new UpdateStoriesRequest
        {
            StoryIds = [action.StoryId]
        };

        var commandResponse = await _repository.Post<UpdateStoriesResponse, UpdateStoriesRequest>(commandRequest, CancellationToken.None);

        if (!commandResponse.SuccessWithValue)
        {
            // TODO: Re-query it????
            return;
        }

        var actionResponse = new UpdateStoryListActionResponse
        {
            ActionId = action.ActionId,
            Errors = [.. commandResponse.Errors],
            Warnings = [.. commandResponse.Warnings],
            Value = commandResponse.Value.FirstOrDefault()
        };

        // TODO: Framework to dispatch general ***something went wrong***

        dispatcher.Dispatch(actionResponse);
    }
}
