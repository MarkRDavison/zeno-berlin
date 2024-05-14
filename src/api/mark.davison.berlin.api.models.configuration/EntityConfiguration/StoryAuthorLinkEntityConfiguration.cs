namespace mark.davison.berlin.api.models.configuration.EntityConfiguration;

public sealed class StoryAuthorLinkEntityConfiguration : BerlinEntityConfiguration<StoryAuthorLink>
{
    protected override bool ConfigureNavigationManually => true;

    public override void ConfigureEntity(EntityTypeBuilder<StoryAuthorLink> builder)
    {
        builder
            .HasOne(_ => _.Story)
            .WithMany(_ => _.StoryAuthorLinks)
            .HasForeignKey(_ => _.StoryId);

        builder
            .HasOne(_ => _.Author)
            .WithMany()
            .HasForeignKey(_ => _.AuthorId);
    }
}
