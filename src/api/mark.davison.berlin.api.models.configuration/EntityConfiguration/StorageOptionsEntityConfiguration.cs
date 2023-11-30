namespace mark.davison.berlin.api.models.configuration.EntityConfiguration;

public class StorageOptionsEntityConfiguration : BerlinEntityConfiguration<StorageOptions>
{
    public override void ConfigureEntity(EntityTypeBuilder<StorageOptions> builder)
    {
        builder
            .Property(_ => _.RetentionAmount);

        builder
            .Property(_ => _.Compressed);
    }
}
