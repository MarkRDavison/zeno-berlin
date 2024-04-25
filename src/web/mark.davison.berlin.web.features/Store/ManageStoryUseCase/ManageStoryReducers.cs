namespace mark.davison.berlin.web.features.Store.ManageStoryUseCase;

public static class ManageStoryReducers
{
    [ReducerMethod]
    public static ManageStoryState FetchManageStoryAction(ManageStoryState state, FetchManageStoryAction action)
    {
        return new ManageStoryState(true, new());
    }

    [ReducerMethod]
    public static ManageStoryState FetchManageStoryActionResponse(ManageStoryState state, FetchManageStoryActionResponse response)
    {
        Console.WriteLine("Fetched manage story data");
        if (response.SuccessWithValue)
        {
            return new ManageStoryState(false, response.Value);
        }

        return new ManageStoryState(false, new());
    }
}
