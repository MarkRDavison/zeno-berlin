using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace mark.davison.berlin.api.persistence;

[ExcludeFromCodeCoverage]
public sealed class BerlinDbContext : DbContextBase<BerlinDbContext>
{
    public BerlinDbContext(DbContextOptions options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if (Database.IsSqlite())
        {
            // SQLite does not have proper support for DateTimeOffset via Entity Framework Core, see the limitations
            // here: https://docs.microsoft.com/en-us/ef/core/providers/sqlite/limitations#query-limitations
            // To work around this, when the Sqlite database provider is used, all model properties of type DateTimeOffset
            // use the DateTimeOffsetToBinaryConverter
            // Based on: https://github.com/aspnet/EntityFrameworkCore/issues/10784#issuecomment-415769754
            // This only supports millisecond precision, but should be sufficient for most use cases.
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var properties = entityType.ClrType.GetProperties().Where(p => p.PropertyType == typeof(DateTimeOffset) || p.PropertyType == typeof(DateTimeOffset?));
                foreach (var property in properties)
                {
                    modelBuilder
                        .Entity(entityType.Name)
                        .Property(property.Name)
                        .HasConversion(new DateTimeOffsetToBinaryConverter());
                }
            }
        }

        modelBuilder
            .ApplyConfigurationsFromAssembly(typeof(UserEntityConfiguration).Assembly)
            .ApplyConfigurationsFromAssembly(typeof(AuthorEntityConfiguration).Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<ExternalLogin> ExternalLogins => Set<ExternalLogin>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Story> Stories => Set<Story>();
    public DbSet<PotentialStory> PotentialStories => Set<PotentialStory>();
    public DbSet<StoryUpdate> StoryUpdates => Set<StoryUpdate>();
    public DbSet<Site> Sites => Set<Site>();
    public DbSet<UpdateType> UpdateTypes => Set<UpdateType>();
    public DbSet<Fandom> Fandoms => Set<Fandom>();
    public DbSet<StoryFandomLink> StoryFandomLinks => Set<StoryFandomLink>();
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<StoryAuthorLink> StoryAuthorLinks => Set<StoryAuthorLink>();
    public DbSet<Job> Jobs => Set<Job>();
}
