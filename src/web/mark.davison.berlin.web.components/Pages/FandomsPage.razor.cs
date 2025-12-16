using mark.davison.berlin.web.components.Forms.AddFandom;

namespace mark.davison.berlin.web.components.Pages;

[StateProperty<FandomListState>]
public partial class FandomsPage : StateComponent
{
    [Inject]
    public required IDialogService DialogService { get; set; }

    [Inject]
    public required IStoreHelper StoreHelper { get; set; }

    [Inject]
    public required IClientNavigationManager ClientNavigationManager { get; set; }

    private IEnumerable<FandomDto> _fandoms => FandomListState.Entities.OrderBy(_ => _.Name);

    protected override async Task OnInitializedAsync()
    {
        await StoreHelper.DispatchAndWaitForResponse<FetchFandomsListAction, FetchFandomsListActionResponse>(new FetchFandomsListAction());
    }

    public async Task OpenAddFandomModal()
    {
        // TODO: Some boiler-plate reducing for this stuff
        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            MaxWidth = MaxWidth.Small,
            FullWidth = true
        };

        var param = new DialogParameters<FormModal<ModalViewModel<AddFandomFormViewModel, AddFandomForm>, AddFandomFormViewModel, AddFandomForm>>
        {
            { _ => _.PrimaryText, "Save" },
            { _ => _.Instance, null }
        };

        var dialog = await DialogService.ShowAsync<FormModal<ModalViewModel<AddFandomFormViewModel, AddFandomForm>, AddFandomFormViewModel, AddFandomForm>>("Add Fandom", param, options);

        await dialog.Result;
    }

    private string? GetFandomName(Guid fandomId)
    {
        return FandomListState.Entities.FirstOrDefault(_ => _.FandomId == fandomId)?.Name;
    }
}
