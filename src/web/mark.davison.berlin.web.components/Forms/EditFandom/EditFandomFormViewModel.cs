namespace mark.davison.berlin.web.components.Forms.EditFandom;

public class EditFandomFormViewModel : IFormViewModel
{
    public string Name { get; set; } = string.Empty;
    public bool IsHidden { get; set; }
    public Guid FandomId { get; set; }

    public bool Valid => !string.IsNullOrEmpty(Name);
}
