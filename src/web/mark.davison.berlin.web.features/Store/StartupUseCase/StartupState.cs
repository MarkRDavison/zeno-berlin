namespace mark.davison.berlin.web.features.Store.StartupUseCase;

public sealed class StartupState : IClientState
{
    public StartupState() : this(true, false, new())
    {

    }

    public StartupState(bool loading, bool loaded, StartupDataDto data)
    {
        Loading = loading;
        Loaded = loaded;
        Data = data;
    }

    public bool Loading { get; }
    public bool Loaded { get; }
    public StartupDataDto Data { get; }
}
