namespace mark.davison.berlin.api.models.configuration.EntityConfiguration;

public class LocationEntityConfiguration : BerlinEntityConfiguration<Location>
{
    public override void ConfigureEntity(EntityTypeBuilder<Location> builder)
    {
        builder
            .Property(_ => _.Path);

        builder
            .Property(_ => _.SharingOptionsId);

        builder
            .Property(_ => _.StorageOptionsId);
    }
}
