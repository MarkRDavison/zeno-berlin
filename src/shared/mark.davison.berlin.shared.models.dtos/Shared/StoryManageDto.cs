namespace mark.davison.berlin.shared.models.dtos.Shared;

public sealed class StoryManageDto
{
    public Guid StoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int CurrentChapters { get; set; }
    public int? TotalChapters { get; set; }
    public int? ConsumedChapters { get; set; }
    public bool Complete { get; set; }
    public bool Favourite { get; set; }
    public Guid UpdateTypeId { get; set; }
    public DateTime LastChecked { get; set; }
    public DateOnly LastAuthored { get; set; }
    public List<Guid> FandomIds { get; set; } = [];
    public List<Guid> AuthorIds { get; set; } = [];

    public List<StoryManageUpdatesDto> Updates { get; set; } = [];

}

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