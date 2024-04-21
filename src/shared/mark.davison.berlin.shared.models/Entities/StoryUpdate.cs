namespace mark.davison.berlin.shared.models.Entities;

public class StoryUpdate : BerlinEntity
{
    public int CurrentChapters { get; set; }
    public int? TotalChapters { get; set; }
    public bool Complete { get; set; }
    public DateOnly LastAuthored { get; set; }
    public Guid StoryId { get; set; }


    public virtual Story? Story { get; set; }
}
