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
}
