namespace mark.davison.berlin.shared.models.dtos.Shared;

public class SerialisedtDataDto
{
    public int Version { get; set; }
    public List<SerialisedStoryDto> Stories { get; set; } = new();
}

public class SerialisedStoryDto
{
    public string StoryAddress { get; set; } = string.Empty;
    public bool Favourite { get; set; }
    public List<SerialisedStoryUpdateDto> Updates { get; set; } = new();
}

public class SerialisedStoryUpdateDto
{
    public int CurrentChapters { get; set; }
    public int? TotalChapters { get; set; }
    public bool Complete { get; set; }
    public DateOnly LastAuthored { get; set; }
}
