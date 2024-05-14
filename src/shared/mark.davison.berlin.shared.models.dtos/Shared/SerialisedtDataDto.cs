﻿namespace mark.davison.berlin.shared.models.dtos.Shared;

public sealed class SerialisedtDataDto
{
    public int Version { get; set; }
    public List<SerialisedStoryDto> Stories { get; set; } = new();
}

public sealed class SerialisedStoryDto
{
    public string StoryAddress { get; set; } = string.Empty;
    public bool Favourite { get; set; }
    public List<SerialisedStoryUpdateDto> Updates { get; set; } = new();
}

public sealed class SerialisedStoryUpdateDto
{
    public int CurrentChapters { get; set; }
    public int? TotalChapters { get; set; }
    public bool Complete { get; set; }
    public DateOnly LastAuthored { get; set; }
}
