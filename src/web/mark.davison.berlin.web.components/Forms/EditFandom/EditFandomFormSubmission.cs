namespace mark.davison.berlin.web.components.Forms.EditFandom;

public sealed class EditFandomFormSubmission : IFormSubmission<EditFandomFormViewModel>
{
    private readonly IStoreHelper _storeHelper;

    public EditFandomFormSubmission(
        IStoreHelper storeHelper
    )
    {
        _storeHelper = storeHelper;
    }

    public async Task<Response> Primary(EditFandomFormViewModel formViewModel)
    {
        var action = new EditFandomListAction
        {
            FandomId = formViewModel.FandomId,
            IsHidden = formViewModel.IsHidden,
            Name = formViewModel.Name,
            ParentFandomId = formViewModel.ParentFandomId
        };

        return await _storeHelper.DispatchAndWaitForResponse<EditFandomListAction, EditFandomListActionResponse>(action);
    }
}
