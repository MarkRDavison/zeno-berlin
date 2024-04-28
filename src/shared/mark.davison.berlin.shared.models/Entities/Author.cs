namespace mark.davison.berlin.shared.models.Entities;

public class Author : BerlinEntity
{
    public string Name { get; set; } = string.Empty;
    public bool IsUserSpecified { get; set; }

    public Guid? SiteId { get; set; }
    public Guid? ParentAuthorId { get; set; }

    public virtual Site? Site { get; set; }

    public virtual Author? ParentAuthor { get; set; }
}
