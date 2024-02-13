namespace mark.davison.berlin.api.models.configuration.EntityConfiguration;

public class StoryEntityConfiguration : BerlinEntityConfiguration<Story>
{
    public override void ConfigureEntity(EntityTypeBuilder<Story> builder)
    {
        builder.Property(_ => _.Name);
        builder.Property(_ => _.Address);
        builder.Property(_ => _.ExternalId);
        builder.Property(_ => _.CurrentChapters);
        builder.Property(_ => _.TotalChapters);
        builder.Property(_ => _.Complete);
    }
}
