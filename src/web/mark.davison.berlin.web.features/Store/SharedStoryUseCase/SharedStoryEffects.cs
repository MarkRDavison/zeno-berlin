namespace mark.davison.berlin.web.features.Store.SharedStoryUseCase;

public sealed class SharedStoryEffects
{
    private readonly IClientHttpRepository _repository;

    public SharedStoryEffects(IClientHttpRepository repository)
    {
        _repository = repository;
    }

    [EffectMethod]
    public async Task HandleSetStoryFavouriteActionAsync(SetStoryFavouriteAction action, IDispatcher dispatcher)
    {
        var manageStoryAction = new SetFavouriteManageStoryAction
        {
            ActionId = action.ActionId,
            StoryId = action.StoryId,
            IsFavourite = action.IsFavourite
        };
        var storyListAction = new SetFavouriteStoryListAction
        {
            ActionId = action.ActionId,
            StoryId = action.StoryId,
            IsFavourite = action.IsFavourite
        };
        var dashboardListAction = new SetFavouriteDashboardListAction
        {
            ActionId = action.ActionId,
            StoryId = action.StoryId,
            IsFavourite = action.IsFavourite
        };

        dispatcher.Dispatch(manageStoryAction);
        dispatcher.Dispatch(storyListAction);
        dispatcher.Dispatch(dashboardListAction);

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


        // TODO: Create response from response, copies Errors/Warnings in common
        var actionResponse = new SetStoryFavouriteActionResponse
        {
            ActionId = action.ActionId,
            Errors = [.. commandResponse.Errors],
            Warnings = [.. commandResponse.Warnings],
            StoryId = action.StoryId,
            IsFavourite = action.IsFavourite
        };

        // TODO: Rollback/revert? from/to?
        if (!commandResponse.SuccessWithValue)
        {
            actionResponse.IsFavourite = !action.IsFavourite;
        }

        // TODO: Framework to dispatch general ***something went wrong***

        dispatcher.Dispatch(actionResponse);
    }

    [EffectMethod]
    public Task HandleSetStoryFavouriteActionResponseAsync(SetStoryFavouriteActionResponse response, IDispatcher dispatcher)
    {
        var manageStoryResponse = new SetFavouriteManageStoryActionResponse
        {
            ActionId = response.ActionId,
            StoryId = response.StoryId,
            IsFavourite = response.IsFavourite
        };
        var storyListResponse = new SetFavouriteStoryListActionResponse
        {
            ActionId = response.ActionId,
            StoryId = response.StoryId,
            IsFavourite = response.IsFavourite
        };
        var dashboardListResponse = new SetFavouriteDashboardListActionResponse
        {
            ActionId = response.ActionId,
            StoryId = response.StoryId,
            IsFavourite = response.IsFavourite
        };

        dispatcher.Dispatch(manageStoryResponse);
        dispatcher.Dispatch(storyListResponse);
        dispatcher.Dispatch(dashboardListResponse);

        return Task.CompletedTask;
    }

    [EffectMethod]
    public async Task HandleSetStoryConsumedChaptersActionAsync(SetStoryConsumedChaptersAction action, IDispatcher dispatcher)
    {
        // TODO: Can only change this from Story page, so no need to trigger other pages if they will just load data on init??
        var manageStoryAction = new SetManageStoryConsumedChaptersAction
        {
            ActionId = action.ActionId,
            StoryId = action.StoryId,
            ConsumedChapters = action.ConsumedChapters
        };

        dispatcher.Dispatch(manageStoryAction);

        var commandRequest = new EditStoryCommandRequest
        {
            StoryId = action.StoryId,
            Changes =
            [
                new DiscriminatedPropertyChangeset
                {
                    Name = nameof(StoryDto.ConsumedChapters),
                    PropertyType = typeof(int?).FullName!,
                    Value = action.ConsumedChapters
                }
            ]
        };

        var commandResponse = await _repository.Post<EditStoryCommandResponse, EditStoryCommandRequest>(commandRequest, CancellationToken.None);

        // TODO: Create response from response, copies Errors/Warnings in common
        var actionResponse = new SetStoryConsumedChaptersActionResponse
        {
            ActionId = action.ActionId,
            Errors = [.. commandResponse.Errors],
            Warnings = [.. commandResponse.Warnings],
            StoryId = action.StoryId,
            ConsumedChapters = action.ConsumedChapters
        };

        if (!commandResponse.Success)
        {
            // TODO: Store original value so we can revert
        }

        dispatcher.Dispatch(actionResponse);
    }

    [EffectMethod]
    public Task HandleSetStoryConsumedChaptersActionResponse(SetStoryConsumedChaptersActionResponse response, IDispatcher dispatcher)
    {
        // TODO: Can only change this from Story page, so no need to trigger other pages if they will just load data on init??
        // TODO: Create response from response, copies Errors/Warnings in common
        var manageStoryResponse = new SetManageStoryConsumedChaptersActionResponse
        {
            ActionId = response.ActionId,
            StoryId = response.StoryId,
            ConsumedChapters = response.ConsumedChapters,
            Errors = [.. response.Errors],
            Warnings = [.. response.Warnings]
        };

        dispatcher.Dispatch(manageStoryResponse);

        return Task.CompletedTask;
    }

    [EffectMethod]
    public async Task HandleDeleteStoryActionAsync(DeleteStoryAction action, IDispatcher dispatcher)
    {
        var storyListAction = new DeleteStoryListAction
        {
            ActionId = action.ActionId,
            StoryId = action.StoryId
        };

        dispatcher.Dispatch(storyListAction);

        var commandRequest = new DeleteStoryCommandRequest
        {
            StoryId = action.StoryId
        };

        var commandResponse = await _repository.Post<DeleteStoryCommandResponse, DeleteStoryCommandRequest>(commandRequest, CancellationToken.None);

        var actionResponse = new DeleteStoryActionResponse
        {
            ActionId = action.ActionId,
            Errors = [.. commandResponse.Errors],
            Warnings = [.. commandResponse.Warnings],
            StoryId = action.StoryId // TODO: Maybe action response holds a copy of the action???
        };

        // TODO: Framework to dispatch general ***something went wrong***

        dispatcher.Dispatch(actionResponse);
    }

    [EffectMethod]
    public Task HandleDeleteStoryActionResponseAsync(DeleteStoryActionResponse response, IDispatcher dispatcher)
    {
        var storyListAction = new DeleteStoryListActionResponse
        {
            ActionId = response.ActionId,
            Errors = [.. response.Errors],
            Warnings = [.. response.Warnings],
            StoryId = response.StoryId
        };

        dispatcher.Dispatch(storyListAction);

        return Task.CompletedTask;
    }

    [EffectMethod]
    public async Task HandleUpdateStoryActionAsync(UpdateStoryAction action, IDispatcher dispatcher)
    {
        // TODO: Naming -> UpdateStoriesCommandRequest
        var commandRequest = new UpdateStoriesRequest
        {
            StoryIds = [action.StoryId]
        };

        var commandResponse = await _repository.Post<UpdateStoriesResponse, UpdateStoriesRequest>(commandRequest, CancellationToken.None);

        var actionResponse = new UpdateStoryActionResponse
        {
            ActionId = action.ActionId,
            Errors = [.. commandResponse.Errors],
            Warnings = [.. commandResponse.Warnings],
            StoryId = action.StoryId,
            Value = commandResponse.Value?.FirstOrDefault()
        };

        // TODO: Framework to dispatch general ***something went wrong***

        dispatcher.Dispatch(actionResponse);
    }

    [EffectMethod]
    public Task HandleUpdateStoryActionResponseAsync(UpdateStoryActionResponse response, IDispatcher dispatcher)
    {
        var manageStoryResponse = new UpdateManageStoryActionResponse
        {
            ActionId = response.ActionId,
            Errors = [.. response.Errors],
            Warnings = [.. response.Warnings],
            StoryId = response.StoryId,
            Value = response.Value
        };

        dispatcher.Dispatch(manageStoryResponse);

        return Task.CompletedTask;
    }

    [EffectMethod]
    public async Task HandleAddStoryListActionAsync(AddStoryAction action, IDispatcher dispatcher)
    {
        var commandRequest = new AddStoryCommandRequest
        {
            StoryAddress = action.StoryAddress,
            UpdateTypeId = action.UpdateTypeId
        };

        var commandResponse = await _repository.Post<AddStoryCommandResponse, AddStoryCommandRequest>(commandRequest, CancellationToken.None);

        // TODO: Create response from response, copies Errors/Warnings in common
        var actionResponse = new AddStoryActionResponse
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
