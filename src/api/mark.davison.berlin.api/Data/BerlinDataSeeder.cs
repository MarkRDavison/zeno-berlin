namespace mark.davison.berlin.api.Data;

[ExcludeFromCodeCoverage]
public sealed class BerlinDataSeeder : IBerlinDataSeeder
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly AppSettings _appSettings;

    public BerlinDataSeeder(
        IServiceScopeFactory serviceScopeFactory,
        IOptions<AppSettings> options
    )
    {
        _serviceScopeFactory = serviceScopeFactory;
        _appSettings = options.Value;
    }

    public async Task EnsureDataSeeded(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IDbContext<BerlinDbContext>>();

        var user = await EnsureUserSeeded(dbContext, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        await EnsureSitesSeeded(dbContext, user, cancellationToken);
        await EnsureUpdateTypesSeeded(dbContext, user, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    internal async Task EnsureSeeded<T>(IDbContext<BerlinDbContext> dbContext, List<T> entities, CancellationToken cancellationToken)
        where T : BerlinEntity, new()
    {
        var existingEntities = await dbContext.Set<T>().Where(_ => _.UserId == Guid.Empty).ToListAsync(cancellationToken);

        var newEntities = entities.Where(_ => !existingEntities.Any(e => e.Id == _.Id)).ToList();

        await dbContext.Set<T>().AddRangeAsync(newEntities, cancellationToken);
    }

    private async Task<User> EnsureUserSeeded(IDbContext<BerlinDbContext> dbContext, CancellationToken cancellationToken)
    {
        var seededUser = new User { Id = Guid.Empty, Email = "berlinsystem@markdavison.kiwi", First = "Berlin", Last = "System", Username = "Berlin.System" };

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
                UserId = user.Id
            }
        };

        if (!_appSettings.PRODUCTION_MODE)
        {
            sites.Add(new Site
            {
                Id = SiteConstants.FakeArchiveOfOurOwn_Id,
                ShortName = SiteConstants.FakeArchiveOfOurOwn_ShortName,
                LongName = SiteConstants.FakeArchiveOfOurOwn_LongName,
                Address = SiteConstants.FakeArchiveOfOurOwn_Address,
                UserId = user.Id
            });
        }

        await EnsureSeeded(dbContext, sites, cancellationToken);
    }

    private async Task EnsureUpdateTypesSeeded(IDbContext<BerlinDbContext> dbContext, User user, CancellationToken cancellationToken)
    {
        var updateTypes = new List<UpdateType> {
            new UpdateType{ Id = UpdateTypeConstants.WhenCompleteId, Description = "When story completed", UserId = user.Id },
            new UpdateType{ Id = UpdateTypeConstants.EachChapterId, Description = "When each chapter is published", UserId = user.Id },
            new UpdateType{ Id = UpdateTypeConstants.MonthlyWithUpdateId, Description = "Monthly as long as at least one update has been made", UserId = user.Id },
        };

        await EnsureSeeded(dbContext, updateTypes, cancellationToken);
    }

}
