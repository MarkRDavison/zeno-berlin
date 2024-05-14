namespace mark.davison.berlin.api.models.configuration.EntityConfiguration;

public sealed class StoryFandomLinkEntityConfiguration : BerlinEntityConfiguration<StoryFandomLink>
{
    protected override bool ConfigureNavigationManually => true;

    public override void ConfigureEntity(EntityTypeBuilder<StoryFandomLink> builder)
    {
        builder
            .HasOne(_ => _.Story)
            .WithMany(_ => _.StoryFandomLinks)
            .HasForeignKey(_ => _.StoryId);

        builder
            .HasOne(_ => _.Fandom)
            .WithMany()
            .HasForeignKey(_ => _.FandomId);
    }
}
