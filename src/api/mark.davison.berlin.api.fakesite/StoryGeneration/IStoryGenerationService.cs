namespace mark.davison.berlin.api.fakesite.StoryGeneration;

public interface IStoryGenerationService
{
    public Task<string> GenerateStoryPage(int externalId, int? externalChapterId, CancellationToken cancellationToken);
}
