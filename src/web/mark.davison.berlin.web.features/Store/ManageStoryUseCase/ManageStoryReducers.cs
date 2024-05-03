namespace mark.davison.berlin.web.features.Store.ManageStoryUseCase;

public static class ManageStoryReducers
{
    [ReducerMethod]
    public static ManageStoryState FetchManageStoryAction(ManageStoryState state, FetchManageStoryAction action)
    {
        return new ManageStoryState(action.SetLoading, action.SetLoading ? new() : state.Data);
    }

    [ReducerMethod]
    public static ManageStoryState FetchManageStoryActionResponse(ManageStoryState state, FetchManageStoryActionResponse response)
    {
        if (response.SuccessWithValue)
        {
            return new ManageStoryState(false, response.Value);
        }

        return new ManageStoryState(false, new());
    }

    [ReducerMethod]
    internal static ManageStoryState SetFavouriteManageStoryAction(ManageStoryState state, SetFavouriteManageStoryAction action)
    {
        if (state.Data.StoryId == action.StoryId)
        {
            state.Data.Favourite = action.IsFavourite;
        }

        return new(state.IsLoading, state.Data);
    }

    [ReducerMethod]
    public static ManageStoryState SetFavouriteManageStoryActionResponse(ManageStoryState state, SetFavouriteManageStoryActionResponse response)
    {
        if (state.Data.StoryId == response.StoryId)
        {
            state.Data.Favourite = response.IsFavourite;
        }

        return new(state.IsLoading, state.Data);
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

        return new(state.IsLoading, state.Data);
    }
}
