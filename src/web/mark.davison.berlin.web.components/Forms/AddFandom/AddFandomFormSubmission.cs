namespace mark.davison.berlin.web.components.Forms.AddFandom;

public sealed class AddFandomFormSubmission : IFormSubmission<AddFandomFormViewModel>
{
    private readonly IStoreHelper _storeHelper;

    public AddFandomFormSubmission(
        IStoreHelper storeHelper
    )
    {
        _storeHelper = storeHelper;
    }

    public async Task<Response> Primary(AddFandomFormViewModel formViewModel)
    {
        var action = new AddFandomListAction
        {
            IsHidden = formViewModel.IsHidden,
            Name = formViewModel.Name,
            ParentFandomId = formViewModel.ParentFandomId
        };

        // TODO: If success navigate to new entity???

        return await _storeHelper.DispatchAndWaitForResponse<AddFandomListAction, AddFandomListActionResponse>(action);
    }
}
