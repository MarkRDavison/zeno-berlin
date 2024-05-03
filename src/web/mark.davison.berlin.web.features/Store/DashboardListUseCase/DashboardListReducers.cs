namespace mark.davison.berlin.web.features.Store.DashboardListUseCase;

public static class DashboardListReducers
{
    [ReducerMethod]
    public static DashboardListState FetchDashboardListAction(DashboardListState state, FetchDashboardListAction action)
    {
        return new DashboardListState(
            true,
            [],
            state.LastLoaded);
    }

    [ReducerMethod]
    public static DashboardListState FetchDashboardListActionResult(DashboardListState state, FetchDashboardListActionResponse response)
    {
        if (response.SuccessWithValue)
        {
            return new DashboardListState(
                false,
                response.Value,
                DateTime.Now);
        }

        return new DashboardListState(
            false,
            [],
            DateTime.MinValue);
    }

    [ReducerMethod]
    public static DashboardListState SetFavouriteDashboardListAction(DashboardListState state, SetFavouriteDashboardListAction action)
    {
        var story = state.Entities.FirstOrDefault(_ => _.StoryId == action.StoryId);

        if (story is not null)
        {
            story.Favourite = action.IsFavourite;
        }

        return new DashboardListState(
            state.IsLoading,
            state.Entities,
            state.LastLoaded);
    }

    [ReducerMethod]
    public static DashboardListState SetFavouriteDashboardListActionResponse(DashboardListState state, SetFavouriteDashboardListActionResponse response)
    {
        var story = state.Entities.FirstOrDefault(_ => _.StoryId == response.StoryId);

        if (story is not null)
        {
            story.Favourite = response.IsFavourite;
        }

        return new DashboardListState(
            state.IsLoading,
            state.Entities,
            state.LastLoaded);
    }

    [ReducerMethod]
    public static DashboardListState AddStoryActionResponse(DashboardListState state, AddStoryActionResponse response)
    {
        if (response.SuccessWithValue)
        {
            var newTile = new DashboardTileDto
            {
                StoryId = response.Value.Id,
                Name = response.Value.Name,
                CurrentChapters = response.Value.CurrentChapters,
                TotalChapters = response.Value.TotalChapters,
                ConsumedChapters = response.Value.ConsumedChapters,
                Complete = response.Value.Complete,
                Favourite = response.Value.Favourite,
                LastAuthored = response.Value.LastAuthored,
                LastChecked = response.Value.LastChecked,
                Fandoms = response.Value.Fandoms
            };
            return new DashboardListState(
                state.IsLoading,
                [.. state.Entities.Where(_ => _.StoryId != newTile.StoryId), newTile],
                state.LastLoaded);
        }
        return state;
    }
}
