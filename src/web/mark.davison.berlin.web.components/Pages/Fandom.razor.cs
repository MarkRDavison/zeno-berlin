namespace mark.davison.berlin.web.components.Pages;

public partial class Fandom
{
    [Parameter]
    public required Guid Id { get; set; }

    public FandomDto Data => FandomListState.Value.Entities.FirstOrDefault(_ => _.FandomId == Id) ?? new();

    public FandomDto? Parent => FandomListState.Value.Entities.FirstOrDefault(_ => _.FandomId == Data.ParentFandomId);

    public IEnumerable<FandomDto> Children => FandomListState.Value.Entities.Where(_ => _.ParentFandomId == Id);

    [Inject]
    public required IState<FandomListState> FandomListState { get; set; }

    [Inject]
    public required IState<StoryListState> StoryListState { get; set; }

    [Inject]
    public required IDispatcher Dispatcher { get; set; }

    [Inject]
    public required IDialogService _dialogService { get; set; }

    private IEnumerable<StoryRowDto> _fandomStories => StoryListState.Value.Stories.Where(_ => _.Fandoms.Any(f => f == Id));


    protected override void OnParametersSet()
    {
        Dispatcher.Dispatch(new FetchStoryListAction
        {
            // TODO: Fandom id
        });
    }

    internal async Task OpenEditFandomModal()
    {
        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            MaxWidth = MaxWidth.Small,
            FullWidth = true
        };

        var param = new DialogParameters<Modal<ModalViewModel<EditFandomFormViewModel, EditFandomForm>, EditFandomFormViewModel, EditFandomForm>>
        {
            { _ => _.PrimaryText, "Save" },
            { _ => _.Instance, new EditFandomFormViewModel
                {
                    Name = Data.Name,
                    IsHidden = Data.IsHidden,
                    FandomId = Id,
                    Fandoms = FandomListState.Value.Entities.Where(_ => _.FandomId != Id && _.ParentFandomId != Id).ToList(),
                    ParentFandomId = Data.ParentFandomId
                }
            }
        };

        var dialog = _dialogService.Show<Modal<ModalViewModel<EditFandomFormViewModel, EditFandomForm>, EditFandomFormViewModel, EditFandomForm>>("EditFandom", param, options);

        await dialog.Result;
    }
}
