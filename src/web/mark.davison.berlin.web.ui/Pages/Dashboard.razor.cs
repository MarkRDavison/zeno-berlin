namespace mark.davison.berlin.web.ui.Pages;

public partial class Dashboard
{
    [Inject]
    public required IDialogService _dialogService { get; set; }

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

        await dialog.Result;
    }
}
