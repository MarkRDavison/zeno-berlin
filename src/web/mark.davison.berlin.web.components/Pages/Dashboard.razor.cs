﻿namespace mark.davison.berlin.web.components.Pages;

public partial class Dashboard
{
    [Inject]
    public required IState<DashboardListState> DashboardListState { get; set; }
    [Inject]
    public required IState<FandomListState> FandomListState { get; set; }
    [Inject]
    public required IState<StartupState> StartupState { get; set; }

    [Inject]
    public required IDialogService DialogService { get; set; }

    [Inject]
    public required IStoreHelper StoreHelper { get; set; }

    [Inject]
    public required IClientNavigationManager ClientNavigationManager { get; set; }

    [Inject]
    public required IDateService DateService { get; set; }

    private readonly List<Guid> _storyIds = new();
    private bool _loaded;

    private IEnumerable<DashboardTileDto> _tiles => _storyIds
        .Select(_ => DashboardListState.Value.Entities.FirstOrDefault(s => s.StoryId == _))
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
        if (!_loaded && !DashboardListState.Value.IsLoading)
        {
            _loaded = true;

            var orderedStories = DashboardListState.Value.Entities
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
        var action = new FetchDashboardListAction();

        using (StoreHelper.ConditionalForce(force))
        {
            await StoreHelper.DispatchWithThrottleAndWaitForResponse<
                FetchDashboardListAction,
                FetchDashboardListActionResponse>(DashboardListState.Value.LastLoaded, action);
        }
    }

    internal async Task OpenAddStoryModal()
    {
        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            MaxWidth = MaxWidth.Small,
            FullWidth = true
        };

        var instance = new AddStoryFormViewModel
        {
            UpdateTypes = [.. StartupState.Value.UpdateTypes]
        };

        var param = new DialogParameters<FormModal<ModalViewModel<AddStoryFormViewModel, AddStoryForm>, AddStoryFormViewModel, AddStoryForm>>
        {
            { _ => _.PrimaryText, "Save" },
            { _ => _.Instance, instance }
        };

        var dialog = DialogService.Show<FormModal<ModalViewModel<AddStoryFormViewModel, AddStoryForm>, AddStoryFormViewModel, AddStoryForm>>("Add Story", param, options);

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

        StoreHelper.Dispatch(new SetStoryFavouriteAction
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
