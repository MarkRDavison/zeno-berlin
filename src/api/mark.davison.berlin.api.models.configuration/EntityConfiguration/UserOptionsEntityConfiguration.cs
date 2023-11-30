namespace mark.davison.berlin.api.models.configuration.EntityConfiguration;

public class UserOptionsEntityConfiguration : BerlinEntityConfiguration<UserOptions>
{
    public override void ConfigureEntity(EntityTypeBuilder<UserOptions> builder)
    {
        builder
            .Property(_ => _.MaxCapacity);

        builder
            .Property(_ => _.IsAdmin);
    }
}
