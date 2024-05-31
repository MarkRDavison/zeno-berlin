namespace mark.davison.berlin.api.models.configuration.EntityConfiguration;

public sealed class PotentialStoryEntityConfiguration : BerlinEntityConfiguration<PotentialStory>
{
    public override void ConfigureEntity(EntityTypeBuilder<PotentialStory> builder)
    {
        builder.Property(_ => _.Address);
        builder.Property(_ => _.Name);
        builder.Property(_ => _.Summary);
    }
}
