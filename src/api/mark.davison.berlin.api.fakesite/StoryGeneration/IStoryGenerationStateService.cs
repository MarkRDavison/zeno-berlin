namespace mark.davison.berlin.api.fakesite.StoryGeneration;

public interface IStoryGenerationStateService
{
    Task ResetAsync();
    StoryGenerationInfo? RecordGeneration(int externalId, int? chapterId);
}
