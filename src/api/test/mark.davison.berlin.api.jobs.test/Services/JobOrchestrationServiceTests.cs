using mark.davison.berlin.api.jobs.Services;
using mark.davison.common.server.abstractions.CQRS;

namespace mark.davison.berlin.api.jobs.test.Services;

[TestClass]
public class JobOrchestrationServiceTests
{

    [TestMethod]
    public void ExtractCommandRequestResponseTypes_WhereObjectGiven_Works()
    {
        var (requestType, responseType) = JobOrchestrationService.ExtractCommandRequestResponseTypes(typeof(TestCommandRequest));

        Assert.AreEqual(requestType, typeof(TestCommandRequest));
        Assert.AreEqual(responseType, typeof(TestCommandResponse));
    }

    [TestMethod]
    public void ConstructCommandHandlerType_CreatesHandlerTypeCorrectly()
    {
        var handlerType = JobOrchestrationService.ConstructCommandHandlerType(typeof(TestCommandRequest), typeof(TestCommandResponse));

        Assert.AreEqual(typeof(ICommandHandler<TestCommandRequest, TestCommandResponse>), handlerType);
    }
}