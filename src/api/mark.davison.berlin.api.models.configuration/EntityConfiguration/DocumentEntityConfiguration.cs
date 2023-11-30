namespace mark.davison.berlin.api.models.configuration.EntityConfiguration;

public class DocumentEntityConfiguration : BerlinEntityConfiguration<Document>
{
    public override void ConfigureEntity(EntityTypeBuilder<Document> builder)
    {
        builder
            .Property(_ => _.Name);

        builder
            .Property(_ => _.FullPath);

        builder
            .Property(_ => _.Version);

        builder
            .Property(_ => _.IsBackup);

        builder
            .Property(_ => _.LocationId);

        builder
            .Property(_ => _.ShareId);

        builder
            .Property(_ => _.SharingOptionsId);

        builder
            .Property(_ => _.StorageOptionsId);
    }
}
