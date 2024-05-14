namespace mark.davison.berlin.web.components.Forms.AddStory;

public sealed class AddStoryFormSubmission : IFormSubmission<AddStoryFormViewModel>
{
    private readonly IStoreHelper _storeHelper;
    private readonly IClientNavigationManager _clientNavigationManager;

    public AddStoryFormSubmission(
        IStoreHelper storeHelper,
        IClientNavigationManager clientNavigationManager)
    {
        _storeHelper = storeHelper;
        _clientNavigationManager = clientNavigationManager;
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

        return response;
    }
}
