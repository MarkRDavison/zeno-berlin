using mark.davison.common.server.abstractions;

namespace mark.davison.berlin.shared.server.common.candidate.test.Persistence;

public static class DbContextHelpers
{
    public static IDbContext<TContext> CreateInMemory<TContext>(
        Func<DbContextOptions, TContext> creator)
        where TContext : DbContextBase<TContext>
    {
        return CreateInMemory<TContext>(creator, $"{Guid.NewGuid()}.db");
    }

    public static IDbContext<TContext> CreateInMemory<TContext>(
        Func<DbContextOptions, TContext> creator,
        string databaseName)
        where TContext : DbContextBase<TContext>
    {
        return creator(
            new DbContextOptionsBuilder<TContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .Options);
    }

    public static void Add<TContext, TEntity>(this IDbContext<TContext> dbContext, TEntity entity)
        where TContext : DbContextBase<TContext>
        where TEntity : BaseEntity
    {
        dbContext.Add([entity]);
    }
    public static void Add<TContext, TEntity>(this IDbContext<TContext> dbContext, IEnumerable<TEntity> entities)
        where TContext : DbContextBase<TContext>
        where TEntity : BaseEntity
    {
        foreach (var e in entities)
        {
            dbContext.Set<TEntity>().Add(e);
        }

        if (dbContext is TContext context)
        {
            context.SaveChanges();
        }
    }
}
