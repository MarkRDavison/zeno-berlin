namespace mark.davison.berlin.api.models.configuration.EntityConfiguration;

public class AuthorEntityConfiguration : BerlinEntityConfiguration<Author>
{
    public override void ConfigureEntity(EntityTypeBuilder<Author> builder)
    {
        builder.Property(_ => _.Name);
        builder.Property(_ => _.IsUserSpecified);
    }
}
