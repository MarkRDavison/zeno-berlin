namespace mark.davison.berlin.shared.models.Entities;

public class StoryFandomLink : BerlinEntity
{
    public Guid StoryId { get; set; }
    public Guid FandomId { get; set; }

    public virtual Story? Story { get; set; }
    public virtual Fandom? Fandom { get; set; }

    public string Name => Fandom?.ParentFandom?.Name ?? Fandom?.Name ?? string.Empty;
}
