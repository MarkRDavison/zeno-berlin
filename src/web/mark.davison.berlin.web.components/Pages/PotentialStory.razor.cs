namespace mark.davison.berlin.web.components.Pages;

[StateProperty<PotentialStoryState>]
public partial class PotentialStory : StateComponent
{
    private bool _inProgress;

    [Parameter]
    public required Guid Id { get; set; }

    [Inject]
    public required IStoreHelper StoreHelper { get; set; }

    [Inject]
    public required IDialogService DialogService { get; set; }

    [Inject]
    public required ISnackbar Snackbar { get; set; }

    [Inject]
    public required IClientNavigationManager ClientNavigationManager { get; set; }

    public PotentialStoryDto? Data => PotentialStoryState.Entities.FirstOrDefault(_ => _.Id == Id);

    protected override async Task OnInitializedAsync()
    {
        var action = new FetchPotentialStoriesAction();

        await StoreHelper.DispatchAndWaitForResponse<
            FetchPotentialStoriesAction,
            FetchPotentialStoriesActionResponse>(action);
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
            { _ => _.Body, $"Are you sure you wish to delete {Data?.Name}?" },
            { _ => _.Color, Color.Error },
            { _ => _.PrimaryCallback, DeletePotentialStory }
        };

        var dialog = await DialogService.ShowAsync<ConfirmationDialog>("Delete story", param, options);

        await dialog.Result;
    }

    private async Task<Response> DeletePotentialStory()
    {
        var action = new DeletePotentialStoryAction { Id = Id };

        var response = await StoreHelper.DispatchAndWaitForResponse<DeletePotentialStoryAction, DeletePotentialStoryActionResponse>(action);

        if (response.Success)
        {
            ClientNavigationManager.NavigateTo(Routes.PotentialStories);
        }

        return response;
    }

    private async Task GrabPotentialStory()
    {
        _inProgress = true;

        var action = new GrabPotentialStoryAction { Id = Id };

        var actionResponse = await StoreHelper.DispatchAndWaitForResponse<GrabPotentialStoryAction, GrabPotentialStoryActionResponse>(action);

        if (actionResponse.SuccessWithValue)
        {
            ClientNavigationManager.NavigateTo(RouteHelpers.Story(actionResponse.Value.Id));
        }
        else
        {
            Snackbar.Add(string.Join(", ", actionResponse.Errors), Severity.Error);
        }

        _inProgress = false;
    }
}
