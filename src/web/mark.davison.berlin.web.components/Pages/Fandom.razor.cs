namespace mark.davison.berlin.web.components.Pages;

[StateProperty<FandomListState>]
[StateProperty<StoryListState>]
public partial class Fandom : StateComponent
{
    [Parameter]
    public required Guid Id { get; set; }

    public FandomDto Data => FandomListState.Entities.FirstOrDefault(_ => _.FandomId == Id) ?? new();

    public FandomDto? Parent => FandomListState.Entities.FirstOrDefault(_ => _.FandomId == Data.ParentFandomId);

    public IEnumerable<FandomDto> Children => FandomListState.Entities.Where(_ => _.ParentFandomId == Id);

    [Inject]
    public required IDispatcher Dispatcher { get; set; }

    [Inject]
    public required IDialogService _dialogService { get; set; }

    private IEnumerable<StoryRowDto> _fandomStories => StoryListState.Stories.Where(_ => _.Fandoms.Any(f => f == Id));


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

        var param = new DialogParameters<FormModal<ModalViewModel<EditFandomFormViewModel, EditFandomForm>, EditFandomFormViewModel, EditFandomForm>>
        {
            { _ => _.PrimaryText, "Save" },
            { _ => _.Instance, new EditFandomFormViewModel
                {
                    Name = Data.Name,
                    IsHidden = Data.IsHidden,
                    FandomId = Id,
                    Fandoms = FandomListState.Entities.Where(_ => _.FandomId != Id && _.ParentFandomId != Id).ToList(),
                    ParentFandomId = Data.ParentFandomId
                }
            }
        };

        var dialog = await _dialogService.ShowAsync<FormModal<ModalViewModel<EditFandomFormViewModel, EditFandomForm>, EditFandomFormViewModel, EditFandomForm>>("Edit fandom", param, options);

        await dialog.Result;
    }
}
