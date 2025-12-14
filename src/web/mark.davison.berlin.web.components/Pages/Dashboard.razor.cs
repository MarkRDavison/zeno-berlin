namespace mark.davison.berlin.web.components.Pages;

[StateProperty<DashboardListState>]
[StateProperty<FandomListState>]
public partial class Dashboard : StateComponent
{
    [Inject]
    public required IStoreHelper StoreHelper { get; set; }

    [Inject]
    public required IClientNavigationManager ClientNavigationManager { get; set; }

    [Inject]
    public required IDateService DateService { get; set; }

    private readonly List<Guid> _storyIds = new();
    private bool _loaded;
    private bool _propagationStopper = false;

    private IEnumerable<DashboardTileDto> _tiles => _storyIds
        .Select(_ => DashboardListState.Entities.FirstOrDefault(s => s.StoryId == _))
        .OfType<DashboardTileDto>();

    protected override async Task OnInitializedAsync()
    {
        await EnsureStateLoaded(true);
    }

    protected override async Task OnParametersSetAsync()
    {
        await EnsureStateLoaded(false);
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (!_loaded && !DashboardListState.IsLoading)
        {
            _loaded = true;

            var orderedStories = DashboardListState.Entities
                .OrderByDescending(_ => _.Favourite)
                .ThenByDescending(_ => _.LastAuthored)
                .ThenByDescending(_ => _.LastChecked)
                .ToList();

            _storyIds.Clear();
            _storyIds.AddRange(orderedStories.Select(_ => _.StoryId));
            InvokeAsync(StateHasChanged);
        }
    }

    private async Task EnsureStateLoaded(bool force)
    {
        await StoreHelper.DispatchAndWaitForResponse<FetchDashboardListAction, FetchDashboardListActionResponse>(new FetchDashboardListAction());
    }

    private async Task OpenAddStoryModal()
    {
        await Task.CompletedTask;
        throw new NotImplementedException();
    }

    private void MudIconClick(Guid storyId, bool set)
    {
        _propagationStopper = true;

        Console.Error.WriteLine("TODO: MudIconClick");
        // TODO
        /*
        StoreHelper.Dispatch(new SetStoryFavouriteAction
        {
            StoryId = storyId,
            IsFavourite = set
        });
        */
    }

    private void CardClick(Guid storyId)
    {
        if (_propagationStopper)
        {
            _propagationStopper = false;
            return;
        }

        ClientNavigationManager.NavigateTo(RouteHelpers.Story(storyId));
    }

    private string StoryCardChapterText(DashboardTileDto tile)
    {
        return $"Chapters: {tile.CurrentChapters}/{tile.TotalChapters?.ToString() ?? "?"}";
    }

    internal string StoryCardUpdatedText(DashboardTileDto tile)
    {
        var humanised = tile.LastAuthored.ToDateTime(TimeOnly.MinValue).Humanize(dateToCompareAgainst: DateService.Today.ToDateTime(TimeOnly.MinValue));
        return $"Updated {(humanised == "now" ? "today" : humanised)}";
    }

    private string StoryCardCheckedText(DashboardTileDto tile)
    {
        return $"Checked {tile.LastChecked.Humanize(dateToCompareAgainst: DateService.Now)}";
    }

    private string StoryCardClasses(DashboardTileDto tile)
    {
        var classes = "story-card pa-2";
        if (tile.ConsumedChapters != null && tile.ConsumedChapters < tile.CurrentChapters)
        {
            classes += " unread";
        }
        return classes;
    }
}
