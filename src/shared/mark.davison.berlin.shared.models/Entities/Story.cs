﻿namespace mark.davison.berlin.shared.models.Entities;

public class Story : BerlinEntity
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string ExternalId { get; set; } = string.Empty;
    public int CurrentChapters { get; set; }
    public int? TotalChapters { get; set; }
    // public int? ConsumedChapters { get; set; } // ??? TODO:  This is how you mark where you are up to???
    public bool Complete { get; set; }
    public bool Favourite { get; set; }
    public DateTime LastChecked { get; set; }
    public DateOnly LastAuthored { get; set; }

    public Guid SiteId { get; set; }
    public Guid UpdateTypeId { get; set; }

    public ICollection<StoryFandomLink> StoryFandomLinks { get; set; } = [];
    public ICollection<StoryAuthorLink> StoryAuthorLinks { get; set; } = [];

    public virtual Site? Site { get; set; }
    public virtual UpdateType? UpdateType { get; set; }

    public StoryDto ToDto()
    {
        return new StoryDto
        {
            Id = Id,
            Name = Name,
            Address = Address,
            ExternalId = ExternalId,
            CurrentChapters = CurrentChapters,
            TotalChapters = TotalChapters,
            Complete = Complete,
            LastChecked = LastChecked,
            LastAuthored = LastAuthored,
            Favourite = Favourite,
            Fandoms = [.. StoryFandomLinks.Select(_ => _.FandomId)],
            Authors = [.. StoryAuthorLinks.Select(_ => _.AuthorId)]
        };
    }
}
