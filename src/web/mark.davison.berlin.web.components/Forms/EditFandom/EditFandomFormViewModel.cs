namespace mark.davison.berlin.web.components.Forms.EditFandom;

public class EditFandomFormViewModel : IFormViewModel
{
    public string Name { get; set; } = string.Empty;
    public bool IsHidden { get; set; }
    public Guid FandomId { get; set; }
    public Guid? ParentFandomId { get; set; }
    public List<FandomDto> Fandoms { get; set; } = [];
    // TODO: Duplicate
    public Task<IEnumerable<Guid?>> Search(string text)
    {
        var upperText = text.ToUpper();
        return Task.FromResult(Fandoms
            .Where(_ => _.Name.ToUpper().Contains(upperText))
            .OrderBy(_ => _.Name)
            .Select(_ => (Guid?)_.FandomId));
    }
    // TODO: Duplicate
    public string FandomIdToString(Guid? id)
    {
        return Fandoms.FirstOrDefault(_ => _.FandomId == id)?.Name ?? string.Empty;
    }

    public bool Valid => !string.IsNullOrEmpty(Name);
}
