namespace mark.davison.berlin.shared.models.dto.Shared;

public sealed record StoryManageDto(
    Guid StoryId,
    string Name,
    string Address,
    int CurrentChapters,
    int? TotalChapters,
    int? ConsumedChapters,
    bool Complete,
    bool Favourite,
    Guid UpdateTypeId,
    DateTime LastChecked,
    DateOnly LastAuthored,
    List<Guid> FandomIds,
    List<Guid> AuthorIds,
    List<StoryManageUpdatesDto> Updates);

public sealed class StoryManageUpdatesDto
{
    public int CurrentChapters { get; set; }
    public int? TotalChapters { get; set; }
    public bool Complete { get; set; }
    public string ChapterAddress { get; set; } = string.Empty;
    public string ChapterTitle { get; set; } = string.Empty;
    public DateOnly LastAuthored { get; set; }
    public DateTime LastChecked { get; set; }
}