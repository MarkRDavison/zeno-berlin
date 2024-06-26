﻿namespace mark.davison.berlin.api.persistence;

[ExcludeFromCodeCoverage]
public sealed class BerlinDbContext : DbContextBase<BerlinDbContext>
{
    public BerlinDbContext(DbContextOptions options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserEntityConfiguration).Assembly);
    }

    public DbSet<Story> Stories => Set<Story>();
    public DbSet<StoryUpdate> StoryUpdates => Set<StoryUpdate>();
    public DbSet<Site> Sites => Set<Site>();
    public DbSet<UpdateType> UpdateTypes => Set<UpdateType>();
    public DbSet<Fandom> Fandoms => Set<Fandom>();
    public DbSet<StoryFandomLink> StoryFandomLinks => Set<StoryFandomLink>();
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<StoryAuthorLink> StoryAuthorLinks => Set<StoryAuthorLink>();
}
