namespace mark.davison.berlin.web.components.Pages;

public partial class FandomsPage
{
    [Inject]
    public required IState<FandomListState> FandomListState { get; set; }

    [Inject]
    public required IDialogService DialogService { get; set; }

    [Inject]
    public required IStoreHelper StoreHelper { get; set; }

    [Inject]
    public required IClientNavigationManager ClientNavigationManager { get; set; }

    private IEnumerable<FandomDto> _fandoms => FandomListState.Value.Entities.OrderBy(_ => _.Name);

    public async Task OpenAddFandomModal()
    {
        // TODO: Some boiler-plate reducing for this stuff
        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            MaxWidth = MaxWidth.Small,
            FullWidth = true
        };

        var param = new DialogParameters<Modal<ModalViewModel<AddFandomFormViewModel, AddFandomForm>, AddFandomFormViewModel, AddFandomForm>>
        {
            { _ => _.PrimaryText, "Save" },
            { _ => _.Instance, null }
        };

        var dialog = DialogService.Show<Modal<ModalViewModel<AddFandomFormViewModel, AddFandomForm>, AddFandomFormViewModel, AddFandomForm>>("Add Fandom", param, options);

        var result = await dialog.Result;

        if (!result.Canceled)
        {
            //TODO: Navigate to newly created Fandom???
        }
    }

    private string? GetFandomName(Guid fandomId)
    {
        return FandomListState.Value.Entities.FirstOrDefault(_ => _.FandomId == fandomId)?.Name;
    }
}
