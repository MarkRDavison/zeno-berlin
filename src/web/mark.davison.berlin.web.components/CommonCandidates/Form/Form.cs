﻿namespace mark.davison.berlin.web.components.CommonCandidates.Form;

public class Form<TFormViewModel> : ComponentBase, IForm<TFormViewModel> where TFormViewModel : IFormViewModel
{
    [Parameter, EditorRequired]
    public required TFormViewModel FormViewModel { get; set; }
}
