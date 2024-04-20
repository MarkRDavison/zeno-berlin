﻿namespace mark.davison.berlin.api.Data;

public class BerlinDataSeeder : IBerlinDataSeeder
{
    protected readonly IServiceScopeFactory _serviceScopeFactory;
    protected readonly AppSettings _appSettings;

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
        var repository = scope.ServiceProvider.GetRequiredService<IRepository>();
        await using (repository.BeginTransaction())
        {
            var user = await EnsureUserSeeded(repository, cancellationToken);
            await EnsureSitesSeeded(repository, user, cancellationToken);
            await EnsureUpdateTypesSeeded(repository, user, cancellationToken);
        }
    }

    internal async Task EnsureSeeded<T>(IRepository repository, List<T> entities, CancellationToken cancellationToken)
        where T : BerlinEntity, new()
    {
        var existingEntities = await repository.GetEntitiesAsync<T>(_ => _.UserId == Guid.Empty, cancellationToken);

        var newEntities = entities.Where(_ => !existingEntities.Any(e => e.Id == _.Id)).ToList();

        await repository.UpsertEntitiesAsync(newEntities, cancellationToken);
    }
    private async Task<User> EnsureUserSeeded(IRepository repository, CancellationToken cancellationToken)
    {
        var seededUser = new User { Id = Guid.Empty, Email = "berlinsystem@markdavison.kiwi", First = "Berlin", Last = "System", Username = "Berlin.System" };

        var existingUser = await repository.GetEntityAsync<User>(_ => _.Id == Guid.Empty, cancellationToken);

        if (existingUser == null)
        {
            existingUser = await repository.UpsertEntityAsync(seededUser, cancellationToken);

            if (existingUser == null)
            {
                throw new InvalidOperationException("Failed to create system user.");
            }
        }

        return existingUser;

    }

    private async Task EnsureSitesSeeded(IRepository repository, User user, CancellationToken cancellationToken)
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

        await EnsureSeeded(repository, sites, cancellationToken);
    }

    private async Task EnsureUpdateTypesSeeded(IRepository repository, User user, CancellationToken cancellationToken)
    {
        var updateTypes = new List<UpdateType> {
            new UpdateType{ Id = UpdateTypeConstants.WhenCompleteId, Description = "When story completed" },
            new UpdateType{ Id = UpdateTypeConstants.EachChapterId, Description = "When each chapter" },
            new UpdateType{ Id = UpdateTypeConstants.MonthlyWithUpdateId, Description = "Monthly as long as at least one update has been made" },
        };

        await EnsureSeeded(repository, updateTypes, cancellationToken);
    }

}
