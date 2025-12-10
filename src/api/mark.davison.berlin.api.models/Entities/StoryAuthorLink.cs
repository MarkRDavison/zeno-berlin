using mark.davison.berlin.api.models.Entities;

namespace mark.davison.berlin.api.models.Entities;

public class StoryAuthorLink : BerlinEntity
{
    public Guid StoryId { get; set; }
    public Guid AuthorId { get; set; }

    public virtual Story? Story { get; set; }
    public virtual Author? Author { get; set; }

    public string Name => Author?.ParentAuthor?.Name ?? Author?.Name ?? string.Empty;
}
