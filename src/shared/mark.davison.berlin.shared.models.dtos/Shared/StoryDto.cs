namespace mark.davison.berlin.shared.models.dtos.Shared;

public class StoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string ExternalId { get; set; } = string.Empty;
    public int CurrentChapters { get; set; }
    public int? TotalChapters { get; set; }
    public int? ConsumedChapters { get; set; }
    public bool Complete { get; set; }
    public Guid UpdateTypeId { get; set; }
    public List<Guid> Fandoms { get; set; } = [];
    public List<Guid> Authors { get; set; } = [];

    public DateTime LastChecked { get; set; }
    public DateOnly LastAuthored { get; set; }

    public bool Favourite { get; set; }

}
