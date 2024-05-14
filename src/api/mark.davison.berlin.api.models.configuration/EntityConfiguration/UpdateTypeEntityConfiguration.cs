namespace mark.davison.berlin.api.models.configuration.EntityConfiguration;

public sealed class UpdateTypeEntityConfiguration : BerlinEntityConfiguration<UpdateType>
{
    public override void ConfigureEntity(EntityTypeBuilder<UpdateType> builder)
    {
        builder.Property(_ => _.Description);
    }
}
