namespace mark.davison.berlin.api.Helpers;

public static class NonProductionHelpers
{
    private static async Task DeleteAll<TEntity>(this IDbContext<BerlinDbContext> dbContext, CancellationToken cancellationToken)
        where TEntity : BaseEntity
    {
        var entities = await dbContext.Set<TEntity>().ToListAsync(cancellationToken);

        foreach (var entity in entities)
        {
            dbContext.Set<TEntity>().Remove(entity);
        }
    }

    public static IEndpointRouteBuilder MapResetEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints
            .MapPost("/api/reset", async (
                HttpContext context,
                [FromServices] IDbContext<BerlinDbContext> dbContext,
                CancellationToken cancellationToken) =>
            {
                await dbContext.DeleteAll<Story>(cancellationToken);
                await dbContext.DeleteAll<StoryUpdate>(cancellationToken);
                await dbContext.DeleteAll<Fandom>(cancellationToken);
                await dbContext.DeleteAll<Author>(cancellationToken);
                await dbContext.DeleteAll<Job>(cancellationToken);

                await dbContext.SaveChangesAsync(cancellationToken);
            })
            .AllowAnonymous();

        return endpoints;
    }
}
