namespace mark.davison.berlin.web.features.Store.StoryListUseCase;

public static class StoryListReducers
{
    [ReducerMethod]
    public static StoryListState FetchStoryListAction(StoryListState state, FetchStoryListAction action)
    {
        return new StoryListState(
            true,
            state.Stories,
            state.LastLoaded);
    }

    [ReducerMethod]
    public static StoryListState FetchStoryListActionResult(StoryListState state, FetchStoryListActionResponse action)
    {
        if (action.SuccessWithValue)
        {
            var stories = action.Value.ToDictionary(_ => _.Id, _ => _);

            foreach (var existingStory in state.Stories.Where(_ => !stories.ContainsKey(_.Id)))
            {
                stories.Add(existingStory.Id, existingStory);
            }

            return new StoryListState(false, stories.Values, DateTime.Now); // TODO: IDateService.Now???
        }

        return new StoryListState(false, state.Stories, DateTime.Now);
    }

    [ReducerMethod]
    public static StoryListState AddStoryAction(StoryListState state, AddStoryListAction action)
    {
        return new StoryListState(
            state.IsLoading,
            [
                .. state.Stories.Where(_ => _.Id != action.ActionId),
                new StoryDto
                {
                    Id = action.ActionId,
                    Address = action.StoryAddress,
                    Temporary = true
                }
            ],
            state.LastLoaded);
    }

    [ReducerMethod]
    public static StoryListState AddStoryActionResponse(StoryListState state, AddStoryListActionResponse action)
    {
        if (action.SuccessWithValue)
        {
            return new StoryListState(
                state.IsLoading,
                [
                    .. state.Stories.Where(_ => _.Id != action.ActionId),
                    action.Value
                ],
            state.LastLoaded);
        }

        return new StoryListState(
            state.IsLoading,
            [
                .. state.Stories.Where(_ => _.Id != action.ActionId)
            ],
            state.LastLoaded);
    }

    [ReducerMethod]
    public static StoryListState DeleteStoryListAction(StoryListState state, DeleteStoryListAction action)
    {
        var storyToDelete = state.Stories.FirstOrDefault(_ => _.Id == action.StoryId);

        if (storyToDelete != null)
        {
            storyToDelete.ClientDeletedActionId = action.ActionId;
        }

        return new StoryListState(
            state.IsLoading,
            state.Stories,
            state.LastLoaded);
    }

    [ReducerMethod]
    public static StoryListState DeleteStoryListActionResponse(StoryListState state, DeleteStoryListActionResponse response)
    {
        if (response.SuccessWithValue)
        {
            return new StoryListState(
                state.IsLoading,
                [.. state.Stories.Where(_ => _.Id != response.Value.DeletedStoryId)],
            state.LastLoaded);
        }
        else
        {
            var failedDeleteStory = state.Stories.FirstOrDefault(_ => _.ClientDeletedActionId == response.ActionId);
            if (failedDeleteStory != null)
            {
                failedDeleteStory.ClientDeletedActionId = null;
            }

            return new StoryListState(
                state.IsLoading,
                state.Stories,
            state.LastLoaded);
        }
    }

    [ReducerMethod]
    public static StoryListState SetFavouriteStoryListAction(StoryListState state, SetFavouriteStoryListAction action)
    {
        var storyToEdit = state.Stories.FirstOrDefault(_ => _.Id == action.StoryId);

        if (storyToEdit != null)
        {
            storyToEdit.Favourite = action.IsFavourite;
        }

        return new StoryListState(
            state.IsLoading,
            state.Stories,
            state.LastLoaded);
    }

    [ReducerMethod]
    public static StoryListState SetFavouriteStoryListActionResponse(StoryListState state, SetFavouriteStoryListActionResponse response)
    {
        if (response.SuccessWithValue)
        {
            return new StoryListState(
                state.IsLoading,
                [.. state.Stories.Where(_ => _.Id != response.Value.Id), response.Value],
            state.LastLoaded);
        }

        return new StoryListState(
            state.IsLoading,
            state.Stories,
            state.LastLoaded);
    }

    [ReducerMethod]
    public static StoryListState UpdateStoryListAction(StoryListState state, UpdateStoryListAction action) => state;

    [ReducerMethod]
    public static StoryListState UpdateStoryListActionResponse(StoryListState state, UpdateStoryListActionResponse response)
    {
        if (response.SuccessWithValue)
        {
            return new StoryListState(
            state.IsLoading,
            [.. state.Stories.Where(_ => _.Id != response.Value.Id), response.Value],
            state.LastLoaded);
        }

        return state;
    }
}
