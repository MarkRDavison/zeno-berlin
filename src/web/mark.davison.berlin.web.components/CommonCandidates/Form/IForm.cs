namespace mark.davison.berlin.web.components.CommonCandidates.Form;

public interface IForm<TFormViewModel> where TFormViewModel : IFormViewModel
{
    TFormViewModel FormViewModel { get; set; }
}
