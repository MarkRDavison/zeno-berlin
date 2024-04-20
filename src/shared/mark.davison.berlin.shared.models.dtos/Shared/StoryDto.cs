﻿namespace mark.davison.berlin.shared.models.dtos.Shared;

public class StoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string ExternalId { get; set; } = string.Empty;
    public int CurrentChapters { get; set; }
    public int? TotalChapters { get; set; }
    public bool Complete { get; set; }

    public Guid SiteId { get; set; }
    public Guid UpdateTypeId { get; set; }

    public DateTime LastModified { get; set; }
    public DateTime LastChecked { get; set; }

    public bool Temporary { get; set; }
    public bool Favourite { get; set; }
    public Guid? ClientDeletedActionId { get; set; } // TODO: Make StoryDto a different type for adding to fluxor state

}