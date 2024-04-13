namespace mark.davison.berlin.api.models.configuration.EntityConfiguration;

public class UpdateTypeEntityConfiguration : BerlinEntityConfiguration<UpdateType>
{
    public override void ConfigureEntity(EntityTypeBuilder<UpdateType> builder)
    {
        builder.Property(_ => _.Description);
    }
}
