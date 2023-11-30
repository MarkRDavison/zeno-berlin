namespace mark.davison.berlin.bff.common.Authentication;

public class BerlinCustomZenoAuthenticationActions : ICustomZenoAuthenticationActions
{
    private readonly IHttpRepository _httpRepository;
    private readonly IDateService _dateService;
    private readonly IOptions<AppSettings> _appSettings;

    public BerlinCustomZenoAuthenticationActions(
        IHttpRepository httpRepository,
        IDateService dateService,
        IOptions<AppSettings> appSettings
    )
    {
        _httpRepository = httpRepository;
        _dateService = dateService;
        _appSettings = appSettings;
    }

    private Task<User?> GetUser(Guid sub, CancellationToken cancellationToken)
    {
        return _httpRepository.GetEntityAsync<User>(
               new QueryParameters { { nameof(User.Sub), sub.ToString() } },
               HeaderParameters.None,
               cancellationToken);
    }

    private async Task<User?> UpsertUser(UserProfile userProfile, string token, CancellationToken cancellationToken)
    {
        return await _httpRepository.UpsertEntityAsync(
                new User
                {
                    Id = Guid.NewGuid(),
                    Sub = userProfile.sub,
                    Admin = false,
                    Created = _dateService.Now,
                    Email = userProfile.email!,
                    First = userProfile.given_name!,
                    Last = userProfile.family_name!,
                    LastModified = _dateService.Now,
                    Username = userProfile.preferred_username!
                },
                HeaderParameters.Auth(token, null),
                cancellationToken);
    }

    private async Task UpsertUserOptions(User user, string token, CancellationToken cancellationToken)
    {
        await _httpRepository.UpsertEntityAsync(
                new UserOptions
                {
                    IsAdmin = user.Username == _appSettings.Value.ADMIN_USERNAME,
                    MaxCapacity = -1,
                    UserId = user.Id
                },
                HeaderParameters.Auth(token, null),
                cancellationToken);
    }

    public async Task<User?> OnUserAuthenticated(UserProfile userProfile, IZenoAuthenticationSession zenoAuthenticationSession, CancellationToken cancellationToken)
    {
        var token = zenoAuthenticationSession.GetString(ZenoAuthenticationConstants.SessionNames.AccessToken);
        var user = await GetUser(userProfile.sub, cancellationToken);

        if (user == null && !string.IsNullOrEmpty(token))
        {
            user = await UpsertUser(userProfile, token, cancellationToken);
        }

        if (user != null)
        {
            await UpsertUserOptions(user, token, cancellationToken);

            zenoAuthenticationSession.SetString(ZenoAuthenticationConstants.SessionNames.User, JsonSerializer.Serialize(user));
        }

        return user;
    }
}
