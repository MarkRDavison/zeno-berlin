namespace mark.davison.berlin.web.components.Forms.AddPotentialStory;

public sealed class AddPotentialStoryFromSubmission : IFormSubmission<AddPotentialStoryFormViewModel>
{
    private readonly IStoreHelper _storeHelper;
    private readonly IClientNavigationManager _clientNavigationManager;
    private readonly ISnackbar _snackbar;

    public AddPotentialStoryFromSubmission(
        IStoreHelper storeHelper,
        IClientNavigationManager clientNavigationManager,
        ISnackbar snackbar)
    {
        _storeHelper = storeHelper;
        _clientNavigationManager = clientNavigationManager;
        _snackbar = snackbar;
    }

    public async Task<Response> Primary(AddPotentialStoryFormViewModel formViewModel)
    {
        var action = new AddPotentialStoryAction
        {
            StoryAddress = formViewModel.StoryAddress
        };

        var response = await _storeHelper.DispatchAndWaitForResponse<AddPotentialStoryAction, AddPotentialStoryActionResponse>(action);

        if (response.SuccessWithValue)
        {
            _clientNavigationManager.NavigateTo(RouteHelpers.PotentialStory(response.Value.Id));
        }

        if (response.Errors.Any(_ => _.Contains(ValidationMessages.DUPLICATE_ENTITY) && _.Contains("Story")))
        {
            _snackbar.Add("You've already added this story!", Severity.Warning);
        }

        return response;
    }
}
