namespace mark.davison.berlin.web.components.Forms.AddStory;

public sealed class AddStoryFormSubmission : IFormSubmission<AddStoryFormViewModel>
{
    private readonly IStoreHelper _storeHelper;
    private readonly IClientNavigationManager _clientNavigationManager;
    private readonly ISnackbar _snackbar;

    public AddStoryFormSubmission(
        IStoreHelper storeHelper,
        IClientNavigationManager clientNavigationManager,
        ISnackbar snackbar)
    {
        _storeHelper = storeHelper;
        _clientNavigationManager = clientNavigationManager;
        _snackbar = snackbar;
    }

    public async Task<Response> Primary(AddStoryFormViewModel formViewModel)
    {
        var action = new AddStoryAction
        {
            StoryAddress = formViewModel.StoryAddress,
            UpdateTypeId = formViewModel.UpdateTypeId
        };

        var response = await _storeHelper.DispatchAndWaitForResponse<AddStoryAction, AddStoryActionResponse>(action);

        if (response.SuccessWithValue)
        {
            _clientNavigationManager.NavigateTo(RouteHelpers.Story(response.Value.Id));
        }

        if (response.Errors.Any(_ => _.Contains(ValidationMessages.DUPLICATE_ENTITY) && _.Contains("Story")))
        {
            _snackbar.Add("You've already added that story!", Severity.Warning);
        }

        return response;
    }
}
