namespace mark.davison.berlin.api.models.configuration.EntityConfiguration;

public abstract class BerlinEntityConfiguration<T> : IEntityTypeConfiguration<T>
    where T : BerlinEntity
{
    public void Configure(EntityTypeBuilder<T> builder)
    {
        builder
            .HasKey(e => e.Id);

        builder
            .Property(e => e.Id)
            .ValueGeneratedNever();

        builder
            .Property(_ => _.Created);
        builder
            .Property(_ => _.LastModified);

        builder
            .HasOne(_ => _.User)
            .WithMany()
            .HasForeignKey(_ => _.UserId);

        if (!ConfigureNavigationManually)
        {
            NavigationPropertyEntityConfigurations.ConfigureEntity(builder);
        }
        ConfigureEntity(builder);
    }

    protected virtual bool ConfigureNavigationManually => false;

    public abstract void ConfigureEntity(EntityTypeBuilder<T> builder);
}