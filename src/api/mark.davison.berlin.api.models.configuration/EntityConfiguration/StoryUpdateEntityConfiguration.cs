namespace mark.davison.berlin.api.models.configuration.EntityConfiguration;

public sealed class StoryUpdateEntityConfiguration : BerlinEntityConfiguration<StoryUpdate>
{
    public override void ConfigureEntity(EntityTypeBuilder<StoryUpdate> builder)
    {
        builder.Property(_ => _.CurrentChapters);
        builder.Property(_ => _.TotalChapters);
        builder.Property(_ => _.Complete);
        builder.Property(_ => _.ChapterAddress);
        builder.Property(_ => _.ChapterTitle);
        builder.Property(_ => _.LastAuthored);
    }
}
