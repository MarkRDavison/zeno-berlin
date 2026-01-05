namespace mark.davison.berlin.shared.models.dto.Shared;

public sealed class SerialisedtDataDto
{
    public int Version { get; set; }
    public List<SerialisedStoryDto> Stories { get; set; } = new();
}

public sealed class SerialisedStoryDto
{
    public string Name { get; set; } = string.Empty;
    public string StoryAddress { get; set; } = string.Empty;
    public bool Favourite { get; set; }
    public int? ConsumedChapters { get; set; }
    public int CurrentChapters { get; set; }
    public int? TotalChapters { get; set; }
    public List<SerialisedStoryUpdateDto> Updates { get; set; } = new();
}

public sealed class SerialisedStoryUpdateDto
{
    public int CurrentChapters { get; set; }
    public int? TotalChapters { get; set; }
    public bool Complete { get; set; }
    public DateOnly LastAuthored { get; set; }
    public string? ChapterAddress { get; set; } = string.Empty;
    public string? ChapterTitle { get; set; } = string.Empty;
}
