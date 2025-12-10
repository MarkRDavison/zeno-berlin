using mark.davison.common.Constants;
using System.Linq.Expressions;

namespace mark.davison.berlin.api.persistence;

public sealed class BerlinDataSeeder : IDataSeeder
{
    private readonly IDateService _dateService;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly bool _isProductionMode;

    public BerlinDataSeeder(
        IDateService dateService,
        IServiceScopeFactory serviceScopeFactory,
        bool isProductionMode
    )
    {
        _dateService = dateService;
        _serviceScopeFactory = serviceScopeFactory;
        _isProductionMode = isProductionMode;
    }
    public async Task SeedDataAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IDbContext<BerlinDbContext>>();

        var user = await EnsureUserSeeded(dbContext, cancellationToken);
        await EnsureTenantSeeded(dbContext, cancellationToken);
        await EnsureRolesSeeded(dbContext, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        await EnsureSitesSeeded(dbContext, user, cancellationToken);
        await EnsureUpdateTypesSeeded(dbContext, user, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    internal async Task EnsureSeeded<T>(IDbContext<BerlinDbContext> dbContext, List<T> entities, CancellationToken cancellationToken)
        where T : BerlinEntity
    {
        var existingEntities = await dbContext.Set<T>().Where(_ => _.UserId == Guid.Empty).ToListAsync(cancellationToken);

        var newEntities = entities.Where(_ => !existingEntities.Any(e => e.Id == _.Id)).ToList();

        await dbContext.Set<T>().AddRangeAsync(newEntities, cancellationToken);
    }

    private async Task EnsureRolesSeeded(IDbContext<BerlinDbContext> dbContext, CancellationToken cancellationToken)
    {
        if (!await ExistsAsync<Role>(dbContext, _ => _.Id == Guid.Parse("02a740de-569f-4477-b5e7-d8622228db17"), cancellationToken))
        {
            await dbContext.AddAsync(new Role
            {
                Id = Guid.Parse("02a740de-569f-4477-b5e7-d8622228db17"),
                Name = RoleConstants.Admin,
                Description = "Administrator with full access",
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                UserId = Guid.Empty
            }, cancellationToken);
        }

        if (!await ExistsAsync<Role>(dbContext, _ => _.Id == Guid.Parse("207af3cb-4a21-4d85-a93d-e16a8690eff2"), cancellationToken))
        {
            await dbContext.AddAsync(new Role
            {
                Id = Guid.Parse("207af3cb-4a21-4d85-a93d-e16a8690eff2"),
                Name = RoleConstants.User,
                Description = "Standard user with limited access",
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                UserId = Guid.Empty
            }, cancellationToken);
        }
    }

    private async Task EnsureTenantSeeded(IDbContext<BerlinDbContext> dbContext, CancellationToken cancellationToken)
    {
        if (!await ExistsAsync<Tenant>(dbContext, _ => _.Id == TenantIds.SystemTenantId, cancellationToken))
        {
            await dbContext.AddAsync(new Tenant
            {
                Id = TenantIds.SystemTenantId,
                Name = "System",
                CreatedAt = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            }, cancellationToken);
        }
    }

    private async Task<User> EnsureUserSeeded(IDbContext<BerlinDbContext> dbContext, CancellationToken cancellationToken)
    {
        var seededUser = new User
        {
            Id = Guid.Empty,
            TenantId = TenantIds.SystemTenantId,
            Email = "berlinsystem@markdavison.kiwi",
            DisplayName = "Berlin System",
            CreatedAt = _dateService.Now,
            LastModified = _dateService.Now
        };

        var existingUser = await dbContext.Set<User>().FindAsync(Guid.Empty, cancellationToken);

        if (existingUser == null)
        {
            await dbContext.AddAsync(seededUser, cancellationToken);
            existingUser = seededUser;
        }

        return existingUser;

    }

    private async Task EnsureSitesSeeded(IDbContext<BerlinDbContext> dbContext, User user, CancellationToken cancellationToken)
    {
        var sites = new List<Site>
        {
            new Site
            {
                Id = SiteConstants.ArchiveOfOurOwn_Id,
                ShortName = SiteConstants.ArchiveOfOurOwn_ShortName,
                LongName = SiteConstants.ArchiveOfOurOwn_LongName,
                Address = SiteConstants.ArchiveOfOurOwn_Address,
                UserId = user.Id,
                Created = _dateService.Now,
                LastModified = _dateService.Now
            }
        };

        if (!_isProductionMode)
        {
            sites.Add(new Site
            {
                Id = SiteConstants.FakeArchiveOfOurOwn_Id,
                ShortName = SiteConstants.FakeArchiveOfOurOwn_ShortName,
                LongName = SiteConstants.FakeArchiveOfOurOwn_LongName,
                Address = SiteConstants.FakeArchiveOfOurOwn_Address,
                UserId = user.Id,
                Created = _dateService.Now,
                LastModified = _dateService.Now
            });
        }

        await EnsureSeeded(dbContext, sites, cancellationToken);
    }

    private async Task EnsureUpdateTypesSeeded(IDbContext<BerlinDbContext> dbContext, User user, CancellationToken cancellationToken)
    {
        var updateTypes = new List<UpdateType> {
            new UpdateType
            {
                Id = UpdateTypeConstants.WhenCompleteId,
                Description = "When story completed",
                UserId = user.Id,
                Created = _dateService.Now,
                LastModified = _dateService.Now
            },
            new UpdateType
            {
                Id = UpdateTypeConstants.EachChapterId,
                Description = "When each chapter is published",
                UserId = user.Id,
                Created = _dateService.Now,
                LastModified = _dateService.Now
            },
            new UpdateType
            {
                Id = UpdateTypeConstants.MonthlyWithUpdateId,
                Description = "Monthly as long as at least one update has been made",
                UserId = user.Id,
                Created = _dateService.Now,
                LastModified = _dateService.Now
            },
        };

        await EnsureSeeded(dbContext, updateTypes, cancellationToken);
    }

    private async Task<bool> ExistsAsync<TEntity>(
        IDbContext<BerlinDbContext> dbContext,
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken token)
        where TEntity : class
    {
        return await dbContext.Set<TEntity>().Where(predicate).AnyAsync(token);
    }

}
