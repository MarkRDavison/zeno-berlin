using mark.davison.berlin.web.features.Store.FandomListUseCase;

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
    public required IDialogService _dialogService { get; set; }

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
            { _ => _.Instance, new EditFandomFormViewModel{ Name = Data.Name, IsHidden = Data.IsHidden, FandomId = Id } }
        };

        var dialog = _dialogService.Show<Modal<ModalViewModel<EditFandomFormViewModel, EditFandomForm>, EditFandomFormViewModel, EditFandomForm>>("EditFandom", param, options);

        await dialog.Result;
    }
}
