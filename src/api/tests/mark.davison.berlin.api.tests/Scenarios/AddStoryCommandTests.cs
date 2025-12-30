namespace mark.davison.berlin.api.tests.Scenarios;

public sealed class AddStoryCommandTests : ApiIntegrationTestBase
{
    [Test]
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
            SiteId = SiteConstants.ArchiveOfOurOwn_Id,
            StoryAddress = "https://archiveofourown.org/works/47216291"
        };

        var response = await handler.Handle(request, currentUserContext, CancellationToken.None);

        await Assert.That(response.SuccessWithValue).IsTrue();
        await Assert.That(response.Value!.Id).IsNotEqualTo(Guid.Empty);

        await Assert.That(response.Value.Authors).Count().IsGreaterThanOrEqualTo(1);
        await Assert.That(response.Value.Fandoms).Count().IsGreaterThanOrEqualTo(1);

        var dbContext = GetRequiredService<IDbContext<BerlinDbContext>>();
        var stories = await dbContext.Set<Story>().Where(_ => _.Id == response.Value.Id).ToListAsync(CancellationToken.None);
        var storyUpdates = await dbContext.Set<StoryUpdate>().Where(_ => _.StoryId == response.Value.Id).ToListAsync(CancellationToken.None);

        await Assert.That(stories).Count().IsEqualTo(1);
        await Assert.That(storyUpdates).Count().IsEqualTo(1);
    }
}