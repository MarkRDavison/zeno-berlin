namespace mark.davison.berlin.api.models.Entities;

public class Fandom : BerlinEntity
{
    public bool IsHidden { get; set; }
    public bool IsUserSpecified { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ExternalName { get; set; } = string.Empty;
    public Guid? ParentFandomId { get; set; }

    public virtual Fandom? ParentFandom { get; set; }
}
