namespace mark.davison.berlin.api.models.configuration.EntityConfiguration;

public class ShareEntityConfiguration : BerlinEntityConfiguration<Share>
{
    public override void ConfigureEntity(EntityTypeBuilder<Share> builder)
    {
        builder
            .Property(_ => _.Name);
    }
}
