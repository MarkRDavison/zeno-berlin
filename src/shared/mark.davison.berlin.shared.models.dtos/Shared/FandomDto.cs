namespace mark.davison.berlin.shared.models.dtos.Shared;

public sealed class FandomDto
{
    public Guid FandomId { get; set; }
    public Guid? ParentFandomId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ExternalName { get; set; } = string.Empty;
    public bool IsHidden { get; set; }
    public bool IsUserSpecified { get; set; }
}
