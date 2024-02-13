namespace mark.davison.berlin.shared.logic.StoryInfo;

public interface IStoryInfoProcessor
{
    string ExtractExternalStoryId(string storyAddress);
    string GenerateBaseStoryAddress(string storyAddress);
    Task<StoryInfoModel> ExtractStoryInfo(string storyAddress, CancellationToken cancellationToken);
}
