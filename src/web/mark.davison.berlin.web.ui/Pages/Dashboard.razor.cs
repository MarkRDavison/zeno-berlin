namespace mark.davison.berlin.web.ui.Pages;

public partial class Dashboard
{
    [Inject]
    public required IState<StoryListState> StoryListState { get; set; }

    [Inject]
    public required IDialogService _dialogService { get; set; }

    [Inject]
    public required IDispatcher Dispatcher { get; set; }

    [Inject]
    public required IClientNavigationManager ClientNavigationManager { get; set; }

    private readonly List<Guid> _storyIds = new();
    private bool _loaded;

    private IEnumerable<StoryDto> _stories => _storyIds
        .Select(_ => StoryListState.Value.Stories.FirstOrDefault(s => s.Id == _))
        .OfType<StoryDto>();

    protected override async Task OnInitializedAsync() // TODO: Framework class for these helpers??? base class???
    {
        await EnsureStateLoaded(true);
    }

    protected override async Task OnParametersSetAsync()
    {
        await Task.CompletedTask;// TODO: Throttle state fetch EnsureStateLoaded(false);
    }

    // TODO: ORRRRRRR Should there be something like, when changing the favourite flag, have 2 fields, one is actual up to date, one is what we sort on and gets synced on init?
    protected override void OnAfterRender(bool firstRender)
    {
        if (!_loaded && !StoryListState.Value.IsLoading)
        {
            _loaded = true;

            var orderedStories = StoryListState.Value.Stories
                .OrderByDescending(_ => _.Favourite)
                .ThenByDescending(_ => _.LastAuthored)
                .ThenByDescending(_ => _.LastChecked)
                .ToList();

            _storyIds.Clear();
            _storyIds.AddRange(orderedStories.Select(_ => _.Id));
            InvokeAsync(StateHasChanged);
        }
    }

    private async Task EnsureStateLoaded(bool force)
    {
        // TODO: state helper to throttle
        await Task.CompletedTask;
        if (force || !StoryListState.Value.Stories.Any())
        {
            Dispatcher.Dispatch(new FetchStoryListAction { Force = force });
        }
        // TODO: awaiter for when the state has actually been updated???
    }

    internal async Task OpenAddStoryModal()
    {
        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            MaxWidth = MaxWidth.Small,
            FullWidth = true
        };

        var param = new DialogParameters<Modal<ModalViewModel<AddStoryFormViewModel, AddStoryForm>, AddStoryFormViewModel, AddStoryForm>>
        {
            { _ => _.PrimaryText, "Save" },
            { _ => _.Instance, null }
        };

        var dialog = _dialogService.Show<Modal<ModalViewModel<AddStoryFormViewModel, AddStoryForm>, AddStoryFormViewModel, AddStoryForm>>("Add Story", param, options);

        var result = await dialog.Result;

        if (!result.Canceled)
        {
            _loaded = false;
            //TODO: Navigate to newly created story???
        }
    }

    private bool _propagationStopper = false;
    private void MudIconClick(Guid storyId, bool set)
    {
        _propagationStopper = true;

        Dispatcher.Dispatch(new SetFavouriteStoryListAction
        {
            StoryId = storyId,
            IsFavourite = set
        });
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

    private string StoryCardChapterText(StoryDto story)
    {
        return $"Chapters: {story.CurrentChapters}/{(story.TotalChapters?.ToString() ?? "?")}";
    }
    private string StoryCardUpdatedText(StoryDto story)
    {
        return $"Updated {story.LastAuthored.Humanize()}";
    }
    private string StoryCardCheckedText(StoryDto story)
    {
        return $"Checked {story.LastChecked.Humanize()}";
    }
}
