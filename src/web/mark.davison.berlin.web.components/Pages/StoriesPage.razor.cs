namespace mark.davison.berlin.web.components.Pages;

public partial class StoriesPage
{
    [Inject]
    public required IState<StoryListState> StoryListState { get; set; }

    [Inject]
    public required IState<FandomListState> FandomListState { get; set; }

    [Inject]
    public required IStoreHelper StoreHelper { get; set; }

    private IEnumerable<StoryRowDto> _stories => StoryListState.Value.Stories.OrderBy(_ => _.Name);

    protected override async Task OnInitializedAsync()
    {
        using (StoreHelper.Force())
        {
            var action = new FetchStoryListAction();
            await StoreHelper.DispatchWithThrottleAndWaitForResponse<
                FetchStoryListAction,
                FetchStoryListActionResponse>(StoryListState.Value.LastLoaded, action);
        }
    }

    private void FavouriteClick(Guid storyId, bool set)
    {
        StoreHelper.Dispatch(new SetStoryFavouriteAction
        {
            StoryId = storyId,
            IsFavourite = set
        });
    }
}
