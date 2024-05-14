namespace mark.davison.berlin.api.models.configuration.EntityConfiguration;

public sealed class SiteEntityConfiguration : BerlinEntityConfiguration<Site>
{
    public override void ConfigureEntity(EntityTypeBuilder<Site> builder)
    {
        builder.Property(_ => _.ShortName);
        builder.Property(_ => _.LongName);
        builder.Property(_ => _.Address);
    }
}
