namespace mark.davison.berlin.web.features.Store.StartupUseCase;

public static class StartupReducers
{
    [ReducerMethod]
    public static StartupState HandleUpdateStartupActionResponse(StartupState state, UpdateStartupActionResponse response)
    {
        if (response.SuccessWithValue)
        {
            return new StartupState(false, true, response.Value);
        }

        return new StartupState(false, false, state.Data);
    }
}
