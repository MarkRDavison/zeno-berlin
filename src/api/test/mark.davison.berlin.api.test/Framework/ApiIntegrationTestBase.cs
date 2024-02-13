namespace mark.davison.berlin.api.test.Framework;

public class ApiIntegrationTestBase : IntegrationTestBase<BerlinApiWebApplicationFactory, AppSettings>
{
    private IServiceScope? _serviceScope;
    public ApiIntegrationTestBase()
    {
        _serviceScope = Services.CreateScope();
        _factory.ModifyCurrentUserContext = (serviceProvider, currentUserContext) =>
        {
            var appSettings = serviceProvider.GetRequiredService<IOptions<AppSettings>>();
            currentUserContext.CurrentUser = CurrentUser;
            currentUserContext.Token = MockJwtTokens.GenerateJwtToken(new[]
            {
                new Claim("sub", CurrentUser.Sub.ToString()),
                new Claim("aud", appSettings.Value.AUTH.CLIENT_ID)
            });
        };
    }

    protected override async Task SeedData(IServiceProvider serviceProvider)
    {
        await base.SeedData(serviceProvider);
        using var scope = Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IRepository>();
        await using (repository.BeginTransaction())
        {
            await repository.UpsertEntityAsync(CurrentUser, CancellationToken.None);
        }
        await SeedTestData();
    }

    protected T GetRequiredService<T>() where T : notnull
    {
        if (_serviceScope == null)
        {
            throw new NullReferenceException();
        }
        return _serviceScope!.ServiceProvider.GetRequiredService<T>();
    }

    protected virtual Task SeedTestData() => Task.CompletedTask;

    protected User CurrentUser { get; } = new User
    {
        Id = Guid.NewGuid(),
        Sub = Guid.NewGuid(),
        Username = "integration.test",
        First = "integration",
        Last = "test",
        Email = "integration.test@gmail.com"
    };
}
