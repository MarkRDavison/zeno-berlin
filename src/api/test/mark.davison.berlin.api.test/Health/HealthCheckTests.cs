namespace mark.davison.berlin.api.test.Health;

[TestClass]
public sealed class HealthCheckTests : ApiIntegrationTestBase
{
    [TestMethod]
    public async Task HealthCheck_ReturnsHealthy()
    {
        var response = await GetRawAsync("/health/readiness");
        Assert.AreEqual("Healthy", response);
        response = await GetRawAsync("/health/liveness");
        Assert.AreEqual("Healthy", response);
        response = await GetRawAsync("/health/startup");
        Assert.AreEqual("Healthy", response);
    }
}
