namespace mark.davison.berlin.api.models.configuration.EntityConfiguration;

public class SharingOptionsEntityConfiguration : BerlinEntityConfiguration<SharingOptions>
{
    public override void ConfigureEntity(EntityTypeBuilder<SharingOptions> builder)
    {
        builder
            .Property(_ => _.Public);
    }
}
