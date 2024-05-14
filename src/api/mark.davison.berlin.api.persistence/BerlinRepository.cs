namespace mark.davison.berlin.api.persistence;

public sealed class BerlinRepository : RepositoryBase<BerlinDbContext>
{
    public BerlinRepository(
        IDbContextFactory<BerlinDbContext> dbContextFactory,
        ILogger<RepositoryBase<BerlinDbContext>> logger
    ) : base(
        dbContextFactory,
        logger)
    {
    }
}
