namespace mark.davison.berlin.web.components.Pages;

public partial class Story
{
    private bool _inProgress;

    [Parameter]
    public required Guid Id { get; set; }

    [Inject]
    public required IState<ManageStoryState> ManageStoryState { get; set; }

    [Inject]
    public required IState<FandomListState> FandomListState { get; set; }

    [Inject]
    public required IState<AuthorListState> AuthorListState { get; set; }

    [Inject]
    public required IState<StartupState> StartupState { get; set; }

    [Inject]
    public required IDispatcher Dispatcher { get; set; }

    [Inject]
    public required IClientNavigationManager ClientNavigationManager { get; set; }

    [Inject]
    public required IDialogService DialogService { get; set; }

    [Inject]
    public required IActionSubscriber ActionSubscriber { get; set; }

    [Inject]
    public required IStoreHelper StoreHelper { get; set; }

    public StoryManageDto Data => ManageStoryState.Value.Data;

    protected override void OnParametersSet()
    {
        if (Id != default && !ManageStoryState.Value.IsLoading && Data.StoryId != Id)
        {
            Dispatcher.Dispatch(new FetchManageStoryAction
            {
                StoryId = Id
            });
        }
    }

    private void FavouriteClick(bool set)
    {
        Dispatcher.Dispatch(new SetStoryFavouriteAction
        {
            StoryId = Id,
            IsFavourite = set
        });
    }

    private async Task AddStoryUpdate()
    {
        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            MaxWidth = MaxWidth.Small,
            FullWidth = true
        };

        var instance = new AddStoryUpdateFormViewModel
        {
            StoryId = Id,
            ExistingUpdates = [.. Data.Updates.Select(_ => new UpdateInfo(_.CurrentChapters, _.LastAuthored))]
        };

        var param = new DialogParameters<Modal<ModalViewModel<AddStoryUpdateFormViewModel, AddStoryUpdateForm>, AddStoryUpdateFormViewModel, AddStoryUpdateForm>>
        {
            { _ => _.PrimaryText, "Save" },
            { _ => _.Instance, instance }
        };

        var dialog = DialogService.Show<Modal<ModalViewModel<AddStoryUpdateFormViewModel, AddStoryUpdateForm>, AddStoryUpdateFormViewModel, AddStoryUpdateForm>>("Add Story Update", param, options);

        var result = await dialog.Result;
    }

    private async Task OpenDeleteConfirmationDialog()
    {
        var options = new DialogOptions // TODO: Standardize/settings service???
        {
            CloseOnEscapeKey = true,
            MaxWidth = MaxWidth.Small,
            FullWidth = false
        };

        var param = new DialogParameters<ConfirmationDialog>
        {
            { _ => _.PrimaryText, "Delete" },
            { _ => _.Body, $"Are you sure you wish to delete {Data.Name}?" },
            { _ => _.Color, Color.Error },
            { _ => _.PrimaryCallback, DeleteStory }
        };

        var dialog = DialogService.Show<ConfirmationDialog>("Delete story", param, options);

        await dialog.Result;
    }

    private async Task<Response> DeleteStory()
    {
        var action = new DeleteStoryAction { StoryId = Id };

        var response = await StoreHelper.DispatchAndWaitForResponse<DeleteStoryAction, DeleteStoryActionResponse>(action);

        if (response.Success)
        {
            ClientNavigationManager.NavigateTo(Routes.Dashboard);
            // TODO: Return to source, you can get here from dashboard or stories page
        }

        return response;
    }

    private async Task CheckStory()
    {
        _inProgress = true;

        var action = new UpdateStoryAction
        {
            StoryId = Id
        };

        Console.Error.WriteLine("TODO: If the story is complete prompt with confirmation, user/server setting to configure this check???");

        await StoreHelper.DispatchAndWaitForResponse<UpdateStoryAction, UpdateStoryActionResponse>(action);

        _inProgress = false;
    }

    private string GetFandomName(Guid fandomId)
    {
        var fandom = FandomListState.Value.Entities.FirstOrDefault(_ => _.FandomId == fandomId);

        return fandom?.Name ?? string.Empty;
    }

    private string GetAuthorName(Guid authorId)
    {
        var author = AuthorListState.Value.Entities.FirstOrDefault(_ => _.AuthorId == authorId);

        return author?.Name ?? string.Empty;
    }

    private string _lastCheckedText => $"Last checked {Data.LastChecked.Humanize()}";
    private string _lastAuthoredText => $"Last authored {Data.LastAuthored.Humanize()}";
    private string _updateTypeText
    {
        get
        {
            var updateType = StartupState.Value.UpdateTypes.FirstOrDefault(_ => _.Id == Data.UpdateTypeId);
            if (updateType == null)
            {
                return string.Empty;
            }

            return $"Updates {updateType.Description.ToLower()}";
        }
    }
    private string _chaptersText => $"Chapters: {Data.CurrentChapters}/{Data.TotalChapters?.ToString() ?? "?"}";

    private string UpdateChapterText(StoryManageUpdatesDto update) => $"{update.CurrentChapters}/{update.TotalChapters?.ToString() ?? "?"}";
}
