﻿namespace mark.davison.berlin.api.models.configuration.EntityConfiguration;

public sealed class StoryEntityConfiguration : BerlinEntityConfiguration<Story>
{
    public override void ConfigureEntity(EntityTypeBuilder<Story> builder)
    {
        builder.Property(_ => _.Name);
        builder.Property(_ => _.Address);
        builder.Property(_ => _.ExternalId);
        builder.Property(_ => _.CurrentChapters);
        builder.Property(_ => _.TotalChapters);
        builder.Property(_ => _.ConsumedChapters);
        builder.Property(_ => _.Complete);
        builder.Property(_ => _.Favourite);
        builder.Property(_ => _.LastChecked);
        builder.Property(_ => _.LastAuthored);

        builder.HasMany(_ => _.StoryFandomLinks).WithOne(_ => _.Story);
    }
}
