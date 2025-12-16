namespace mark.davison.berlin.web.features.Store.PotentialStoryUseCase;

public static class PotentialStoryReducers
{
    [ReducerMethod]
    public static PotentialStoryState FetchPotentialStoriesAction(PotentialStoryState state, FetchPotentialStoriesAction action)
    {
        return new PotentialStoryState(
            true,
            []);
    }

    [ReducerMethod]
    public static PotentialStoryState FetchPotentialStoriesActionResponse(PotentialStoryState state, FetchPotentialStoriesActionResponse response)
    {
        if (response.SuccessWithValue)
        {
            var returnedIds = response.Value.Select(_ => _.Id).ToHashSet();

            return new PotentialStoryState(
                false,
                [.. state.Entities.Where(_ => !returnedIds.Contains(_.Id)), .. response.Value]);
        }

        return new PotentialStoryState(
            false,
            []);
    }

    [ReducerMethod]
    public static PotentialStoryState AddPotentialStoryActionResponse(PotentialStoryState state, AddPotentialStoryActionResponse response)
    {
        if (response.SuccessWithValue)
        {
            return new PotentialStoryState(
                state.IsLoading,
                [.. state.Entities.Where(_ => _.Id != response.Value.Id), response.Value]);
        }

        return state;
    }

    [ReducerMethod]
    public static PotentialStoryState DeletetPotentialStoryActionResponse(PotentialStoryState state, DeletePotentialStoryActionResponse response)
    {
        if (response.SuccessWithValue)
        {
            return new PotentialStoryState(
                state.IsLoading,
                [.. state.Entities.Where(_ => _.Id != response.Value)]);
        }

        return state;
    }

    [ReducerMethod]
    public static PotentialStoryState GrabPotentialStoryActionResponse(PotentialStoryState state, GrabPotentialStoryActionResponse response)
    {
        if (response.SuccessWithValue)
        {
            return new PotentialStoryState(
                state.IsLoading,
                [.. state.Entities.Where(_ => _.Id != response.Value.Id)]);
        }

        return state;
    }
}