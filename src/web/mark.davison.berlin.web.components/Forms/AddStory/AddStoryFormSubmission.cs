namespace mark.davison.berlin.web.components.Forms.AddStory;

public class AddStoryFormSubmission : IFormSubmission<AddStoryFormViewModel>
{
    private readonly IStoreHelper _storeHelper;

    public AddStoryFormSubmission(
        IStoreHelper storeHelper
    )
    {
        _storeHelper = storeHelper;
    }

    public async Task<Response> Primary(AddStoryFormViewModel formViewModel)
    {
        var action = new AddStoryListAction
        {
            StoryAddress = formViewModel.StoryAddress
        };

        return await _storeHelper.DispatchAndWaitForResponse<AddStoryListAction, AddStoryListActionResponse>(action);
    }
}
