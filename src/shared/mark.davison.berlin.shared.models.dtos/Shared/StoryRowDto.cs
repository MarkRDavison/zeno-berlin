namespace mark.davison.berlin.shared.models.dtos.Shared;

public class StoryRowDto
{
    public Guid StoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsFavourite { get; set; }
    public bool IsComplete { get; set; }
    public Guid UpdateTypeId { get; set; }
    public int CurrentChapters { get; set; }
    public int? TotalChapters { get; set; }
    public int? ConsumedChapters { get; set; }
    public List<Guid> Fandoms { get; set; } = [];
    public List<Guid> Authors { get; set; } = [];
    public DateOnly LastAuthored { get; set; }
}
