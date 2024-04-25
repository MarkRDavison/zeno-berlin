namespace mark.davison.berlin.web.components.Pages;

public partial class StoriesPage
{
    [Inject]
    public required IState<StoryListState> StoryListState { get; set; }

    [Inject]
    public required IStoreHelper StoreHelper { get; set; }

    protected override async Task OnInitializedAsync() // TODO: Framework class for these helpers??? base class???
    {
        await EnsureStateLoaded(true);
    }

    protected override async Task OnParametersSetAsync()
    {
        await EnsureStateLoaded(false);
    }

    private async Task EnsureStateLoaded(bool force)
    {
        var action = new FetchStoryListAction();
        if (force)
        {
            using (StoreHelper.Force())
            {
                await StoreHelper.DispatchWithThrottleAndWaitForResponse<
                    FetchStoryListAction,
                    FetchStoryListActionResponse>(StoryListState.Value.LastLoaded, action);
            }
        }
        else
        {
            await StoreHelper.DispatchWithThrottleAndWaitForResponse<
                FetchStoryListAction,
                FetchStoryListActionResponse>(StoryListState.Value.LastLoaded, action);
        }
    }
}
