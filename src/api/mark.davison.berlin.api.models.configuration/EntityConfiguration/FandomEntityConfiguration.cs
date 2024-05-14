namespace mark.davison.berlin.api.models.configuration.EntityConfiguration;

public sealed class FandomEntityConfiguration : BerlinEntityConfiguration<Fandom>
{
    public override void ConfigureEntity(EntityTypeBuilder<Fandom> builder)
    {
        builder.Property(_ => _.IsHidden);
        builder.Property(_ => _.IsUserSpecified);
        builder.Property(_ => _.Name);
        builder.Property(_ => _.ExternalName);
    }
}
