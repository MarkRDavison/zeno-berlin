namespace mark.davison.berlin.web.ui.Pages;

public partial class Story
{
    private bool _inProgress;
    [Parameter]
    public required Guid Id { get; set; }

    [Inject]
    public required IState<StoryListState> StoryListState { get; set; }

    [Inject]
    public required IDispatcher Dispatcher { get; set; }

    [Inject]
    public required IClientNavigationManager ClientNavigationManager { get; set; }

    [Inject]
    public required IDialogService DialogService { get; set; }

    [Inject]
    public required IActionSubscriber ActionSubscriber { get; set; }

    public StoryDto CurrentStory => StoryListState.Value.Stories.First(_ => _.Id == Id);
    private void FavouriteClick(bool set)
    {
        Dispatcher.Dispatch(new SetFavouriteStoryListAction
        {
            StoryId = Id,
            IsFavourite = set
        });
    }

    private async Task OpenDeleteConfirmationDialog()
    {
        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            MaxWidth = MaxWidth.Small,
            FullWidth = false
        };

        var param = new DialogParameters<ConfirmationDialog>
        {
            { _ => _.PrimaryText, "Delete" },
            { _ => _.Body, $"Are you sure you wish to delete {CurrentStory.Name}?" },
            { _ => _.Color, Color.Error },
            { _ => _.PrimaryCallback, DeleteStory }
        };

        var dialog = DialogService.Show<ConfirmationDialog>("Delete story", param, options);

        await dialog.Result;
    }

    private async Task<Response> DeleteStory()
    {
        var action = new DeleteStoryListAction { StoryId = Id };

        // TODO: Framework-itize this.
        TaskCompletionSource tcs = new();
        DeleteStoryListActionResponse? response = null;

        ActionSubscriber.SubscribeToAction(
            this,
            (DeleteStoryListActionResponse resultAction) =>
            {
                response = resultAction;
                tcs.SetResult();
            });

        using (ActionSubscriber.GetActionUnsubscriberAsIDisposable(this))
        {
            Dispatcher.Dispatch(action);

            await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(100))); // TODO: Configureable
        }

        if (response == null)
        {
            return new Response { Errors = ["TODO: TIMED OUT, please try again..."] };
        }

        if (response.Success)
        {
            ClientNavigationManager.NavigateTo(Routes.Dashboard);
        }

        return response;
    }

    private async Task CheckStory()
    {
        _inProgress = true;

        var action = new UpdateStoryListAction
        {
            StoryId = Id
        };

        Console.Error.WriteLine("TODO: If the story is complete prompt with confirmation, user/server setting to configure this check???");

        // TODO: Framework-itize this.
        TaskCompletionSource tcs = new();
        UpdateStoryListActionResponse? response = null;

        ActionSubscriber.SubscribeToAction(
            this,
            (UpdateStoryListActionResponse resultAction) =>
            {
                response = resultAction;
                tcs.SetResult();
            });

        using (ActionSubscriber.GetActionUnsubscriberAsIDisposable(this))
        {
            Dispatcher.Dispatch(action);

            await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(100))); // TODO: Configureable
        }

        _inProgress = false;

        if (response == null)
        {
            Console.WriteLine("TODO: TIMED OUT, please try again...");
            return;
        }

        await InvokeAsync(StateHasChanged);
    }

    private string _lastCheckedText => $"Last checked {CurrentStory.LastChecked.Humanize()}";
    private string _chaptersText => $"Chapters: {CurrentStory.CurrentChapters}/{(CurrentStory.TotalChapters?.ToString() ?? "?")}";
}
