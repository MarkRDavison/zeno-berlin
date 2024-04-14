namespace mark.davison.berlin.web.components.CommonCandidates.Form;

public interface IFormSubmission<TFormViewModel> where TFormViewModel : IFormViewModel
{
    Task<Response> Primary(TFormViewModel formViewModel);
}
