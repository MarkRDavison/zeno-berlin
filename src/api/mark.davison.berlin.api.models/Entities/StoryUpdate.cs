namespace mark.davison.berlin.api.models.Entities;

public class StoryUpdate : BerlinEntity
{
    public int CurrentChapters { get; set; }
    public int? TotalChapters { get; set; }
    public bool Complete { get; set; }
    public string? ChapterAddress { get; set; }
    public string? ChapterTitle { get; set; }
    public DateOnly LastAuthored { get; set; }
    public Guid StoryId { get; set; }


    public virtual Story? Story { get; set; }
}
