namespace mark.davison.berlin.web.features.Store.ManageStoryUseCase;

public static class ManageStoryReducers
{
    [ReducerMethod]
    public static ManageStoryState FetchManageStoryAction(ManageStoryState state, FetchManageStoryAction action)
    {
        return new ManageStoryState(
            action.SetLoading,
            state.Data);
    }

    [ReducerMethod]
    public static ManageStoryState FetchManageStoryActionResponse(ManageStoryState state, FetchManageStoryActionResponse response)
    {
        if (response.SuccessWithValue)
        {
            return new ManageStoryState(false, response.Value);
        }

        return state;
    }

    [ReducerMethod]
    internal static ManageStoryState SetFavouriteManageStoryAction(ManageStoryState state, SetFavouriteManageStoryAction action)
    {
        if (state.Data.StoryId == action.StoryId)
        {
            return new(state.IsLoading, state.Data with
            {
                Favourite = action.IsFavourite
            });
        }

        return state;
    }

    [ReducerMethod]
    public static ManageStoryState SetFavouriteManageStoryActionResponse(ManageStoryState state, SetFavouriteManageStoryActionResponse response)
    {
        if (state.Data.StoryId == response.StoryId)
        {
            return new(state.IsLoading, state.Data with
            {
                Favourite = response.IsFavourite
            });
        }

        return state;
    }

    [ReducerMethod]
    internal static ManageStoryState SetManageStoryConsumedChaptersAction(ManageStoryState state, SetManageStoryConsumedChaptersAction action)
    {
        if (state.Data.StoryId == action.StoryId)
        {
            return new(state.IsLoading, state.Data with
            {
                ConsumedChapters = action.ConsumedChapters
            });
        }

        return state;
    }

    [ReducerMethod]
    internal static ManageStoryState SetManageStoryConsumedChaptersActionResponse(ManageStoryState state, SetManageStoryConsumedChaptersActionResponse response)
    {
        if (state.Data.StoryId == response.StoryId)
        {
            return new(state.IsLoading, state.Data with
            {
                ConsumedChapters = response.ConsumedChapters
            });
        }

        return state;
    }

    [ReducerMethod]
    public static ManageStoryState AddManageStoryUpdateActionResponse(ManageStoryState state, AddManageStoryUpdateActionResponse response)
    {
        if (response.SuccessWithValue &&
            state.Data.StoryId == response.Value.StoryId)
        {
            state.Data.Updates.Add(new StoryManageUpdatesDto
            {
                CurrentChapters = response.Value.CurrentChapters,
                TotalChapters = response.Value.TotalChapters,
                Complete = response.Value.Complete,
                LastAuthored = response.Value.UpdateDate,
                LastChecked = new DateTime(response.Value.UpdateDate, TimeOnly.MinValue)
            });
        }

        return state;
    }
}
