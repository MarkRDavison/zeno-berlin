namespace mark.davison.berlin.web.components.Forms.AddStoryUpdate;

public class AddStoryUpdateFormSubmission : IFormSubmission<AddStoryUpdateFormViewModel>
{
    private readonly IStoreHelper _storeHelper;

    public AddStoryUpdateFormSubmission(IStoreHelper storeHelper)
    {
        _storeHelper = storeHelper;
    }

    public async Task<Response> Primary(AddStoryUpdateFormViewModel formViewModel)
    {
        var action = new AddManageStoryUpdateAction
        {
            StoryId = formViewModel.StoryId,
            CurrentChapters = formViewModel.CurrentChapters!.Value,
            TotalChapters = formViewModel.TotalChapters,
            Complete = formViewModel.Complete,
            UpdateDate = DateOnly.FromDateTime(formViewModel.UpdateDate!.Value)
        };

        return await _storeHelper.DispatchAndWaitForResponse<AddManageStoryUpdateAction, AddManageStoryUpdateActionResponse>(action);
    }
}
