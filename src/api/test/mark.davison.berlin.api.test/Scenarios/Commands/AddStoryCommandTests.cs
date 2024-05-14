namespace mark.davison.berlin.api.test.Scenarios.Commands;

[TestClass]
public sealed class AddStoryCommandTests : ApiIntegrationTestBase
{
    [TestMethod]
    public async Task AddStoryWorks()
    {
        _factory
            .GetMessageHandler(nameof(Ao3StoryInfoProcessor))
            .Callback = async (request) =>
            {
                return new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(await File.ReadAllTextAsync("TestData/ExampleAo3WorkResponse_1.html"))
                };
            };

        var handler = GetRequiredService<ICommandHandler<AddStoryCommandRequest, AddStoryCommandResponse>>();
        var currentUserContext = GetRequiredService<ICurrentUserContext>();
        var request = new AddStoryCommandRequest
        {
            StoryAddress = "https://archiveofourown.org/works/47216291"
        };

        var response = await handler.Handle(request, currentUserContext, CancellationToken.None);

        Assert.IsTrue(response.SuccessWithValue);
        Assert.AreNotEqual(Guid.Empty, response.Value.Id);

        var repository = GetRequiredService<IReadonlyRepository>();
        await using (repository.BeginTransaction())
        {
            var stories = await repository.GetEntitiesAsync<Story>(_ => _.Id == response.Value.Id, CancellationToken.None);
            var storyUpdates = await repository.GetEntitiesAsync<StoryUpdate>(_ => _.StoryId == response.Value.Id, CancellationToken.None);

            Assert.AreEqual(1, stories.Count);
            Assert.AreEqual(1, storyUpdates.Count);
        }
    }
}
