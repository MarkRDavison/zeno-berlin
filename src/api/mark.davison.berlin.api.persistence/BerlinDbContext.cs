namespace mark.davison.berlin.api.persistence;

[ExcludeFromCodeCoverage]
public sealed class BerlinDbContext : DbContextBase<BerlinDbContext>
{
    public BerlinDbContext(DbContextOptions options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                var type = property.ClrType;

                if (type == typeof(DateTime) || type == typeof(DateTime?))
                {
                    property.SetColumnType("timestamp(0) without time zone");
                }
                else if (type == typeof(TimeOnly) || type == typeof(TimeOnly?))
                {
                    property.SetColumnType("time(0) without time zone");
                }
                else if (type == typeof(DateOnly) || type == typeof(DateOnly?))
                {
                    property.SetColumnType("date");
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
