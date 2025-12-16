namespace mark.davison.berlin.web.components.Pages;

[StateProperty<PotentialStoryState>]
public partial class PotentialStories : StateComponent
{

    [Inject]
    public required IStoreHelper StoreHelper { get; set; }

    [Inject]
    public required IDialogService DialogService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var action = new FetchPotentialStoriesAction();

        await StoreHelper.DispatchAndWaitForResponse<
            FetchPotentialStoriesAction,
            FetchPotentialStoriesActionResponse>(action);
    }

    private async Task OpenAddPotentialStoryModal()
    {
        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            MaxWidth = MaxWidth.Small,
            FullWidth = true
        };

        var param = new DialogParameters<FormModal<ModalViewModel<AddPotentialStoryFormViewModel, AddPotentialStoryForm>, AddPotentialStoryFormViewModel, AddPotentialStoryForm>>
        {
            { _ => _.PrimaryText, "Save" },
            { _ => _.Instance, null }
        };

        var dialog = await DialogService.ShowAsync<FormModal<ModalViewModel<AddPotentialStoryFormViewModel, AddPotentialStoryForm>, AddPotentialStoryFormViewModel, AddPotentialStoryForm>>("Add Potential Story", param, options);

        await dialog.Result;
    }
}