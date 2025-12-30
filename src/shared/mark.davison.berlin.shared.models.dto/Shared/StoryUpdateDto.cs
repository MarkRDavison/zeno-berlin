namespace mark.davison.berlin.shared.models.dto.Shared;

public sealed class StoryUpdateDto
{
    public Guid StoryId { get; set; }
    public int CurrentChapters { get; set; }
    public int? TotalChapters { get; set; }
    public bool Complete { get; set; }
    public DateOnly UpdateDate { get; set; }
}
