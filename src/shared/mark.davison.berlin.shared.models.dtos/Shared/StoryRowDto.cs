namespace mark.davison.berlin.shared.models.dtos.Shared;

public class StoryRowDto
{
    public Guid StoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Author { get; set; } = "TODO";
    public bool IsFavourite { get; set; }
    public bool IsComplete { get; set; }
    public int CurrentChapters { get; set; }
    public int? TotalChapters { get; set; }
    public List<Guid> Fandoms { get; set; } = [];
}
