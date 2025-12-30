namespace mark.davison.berlin.web.components.Pages;

[StateProperty<StoryListState>]
[StateProperty<FandomListState>]
[StateProperty<AuthorListState>]
public partial class StoriesPage : StateComponent
{
    [Inject]
    public required IStoreHelper StoreHelper { get; set; }

    private IEnumerable<StoryRowDto> _stories => StoryListState.Stories.OrderBy(_ => _.Name);

    protected override async Task OnInitializedAsync()
    {
        await StoreHelper.DispatchAndWaitForResponse<
            FetchStoryListAction, FetchStoryListActionResponse>(
            new FetchStoryListAction());
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
