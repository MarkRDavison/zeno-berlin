namespace mark.davison.berlin.shared.models.Entities;

public class Story : BerlinEntity
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string ExternalId { get; set; } = string.Empty;
    public int CurrentChapters { get; set; }
    public int? TotalChapters { get; set; }
    public bool Complete { get; set; }

    public Guid SiteId { get; set; }


    public virtual Site? Site { get; set; }
}
