using mark.davison.berlin.api.models.Entities;
using mark.davison.berlin.api.persistence;
using mark.davison.berlin.api.queries.Scenarios.StartupQuery;
using mark.davison.berlin.shared.models.dto.Scenarios.Queries.Startup;
using mark.davison.common.authentication.server.abstractions.Services;
using mark.davison.common.authentication.server.Configuration;
using mark.davison.common.persistence;
using mark.davison.common.server.test.Persistence;
using Microsoft.Extensions.Options;
using Moq;

namespace mark.davison.berlin.api.queries.tests.Scenarios.StartupQuery;

public sealed class StartupQueryProcessorTests
{
    private readonly Guid _currentUserId = Guid.NewGuid();

    private Mock<ICurrentUserContext> _currentUserContext = default!;

    private IDbContext<BerlinDbContext> _dbContext = default!;
    private StartupQueryProcessor _processor = default!;
    private readonly AuthenticationSettings _authSettings = new();

    [Before(Test)]
    public void BeforeTest()
    {
        _currentUserContext = new();
        _currentUserContext
            .Setup(_ => _.UserId)
            .Returns(_currentUserId);

        _dbContext = DbContextHelpers.CreateInMemory<BerlinDbContext>(_ => new(_));

        _processor = new StartupQueryProcessor(
            _dbContext,
            Options.Create(_authSettings));
    }

    [Test]
    public async Task ProcessAsync_ReturnsExpectedData()
    {
        _authSettings.PROVIDERS.Add(new AuthenticationProviderConfiguration { Name = "Keycloak", Type = "oidc" });
        _authSettings.PROVIDERS.Add(new AuthenticationProviderConfiguration { Name = "Google", Type = "oidc" });

        List<string> updateTypes = ["Some description", "Another description"];

        foreach (var ut in updateTypes)
        {
            _dbContext.AddSync(new UpdateType
            {
                Id = Guid.NewGuid(),
                Description = ut,
                UserId = Guid.Empty,
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            });
        }

        var request = new StartupQueryRequest();
        var response = await _processor.ProcessAsync(request, _currentUserContext.Object, CancellationToken.None);

        await Assert.That(response.SuccessWithValue).IsTrue();
        await Assert.That(response.Value!.EnabledAuthProviders).IsEquivalentTo(_authSettings.PROVIDERS.Select(_ => _.Name));
        await Assert.That(response.Value!.UpdateTypes.Select(_ => _.Description)).IsEquivalentTo(updateTypes);
    }
}
