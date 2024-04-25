namespace mark.davison.berlin.web.features.Store.FandomListUseCase;

public static class FandomListReducers
{
    [ReducerMethod]
    public static FandomListState FetchFandomsListAction(FandomListState state, FetchFandomsListAction action)
    {
        return new FandomListState(
            true,
            []);
    }

    [ReducerMethod]
    public static FandomListState FetchFandomsListActionResponse(FandomListState state, FetchFandomsListActionResponse response)
    {
        if (response.SuccessWithValue)
        {
            return new FandomListState(
                false,
                response.Value);
        }

        return new FandomListState(
            false,
            []);
    }

    [ReducerMethod]
    public static FandomListState EditFandomListAction(FandomListState state, EditFandomListAction action)
    {
        return state;
    }

    [ReducerMethod]
    public static FandomListState EditFandomListActionResponse(FandomListState state, EditFandomListActionResponse response)
    {
        if (response.SuccessWithValue)
        {
            return new FandomListState(
                state.IsLoading,
                [.. state.Entities.Where(_ => _.FandomId != response.Value.FandomId), response.Value]);
        }

        return state;
    }

    [ReducerMethod]
    public static FandomListState AddFandomListActionResponse(FandomListState state, AddFandomListActionResponse response)
    {

        if (response.SuccessWithValue)
        {
            return new FandomListState(
                state.IsLoading,
                [.. state.Entities.Where(_ => _.FandomId != response.Value.FandomId), response.Value]);
        }

        return state;
    }
}
