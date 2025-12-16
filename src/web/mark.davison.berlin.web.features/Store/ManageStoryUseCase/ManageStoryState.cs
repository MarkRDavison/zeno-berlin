namespace mark.davison.berlin.web.features.Store.ManageStoryUseCase;

public sealed class ManageStoryState : IClientState
{
    public bool IsLoading { get; }
    public StoryManageDto Data { get; }

    public ManageStoryState() : this(false, new())
    {

    }

    public ManageStoryState(bool isLoading, StoryManageDto data)
    {
        IsLoading = isLoading;
        Data = data;
    }
}
