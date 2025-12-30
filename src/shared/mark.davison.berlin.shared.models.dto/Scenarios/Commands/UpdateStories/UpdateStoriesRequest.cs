namespace mark.davison.berlin.shared.models.dto.Scenarios.Commands.UpdateStories;

[PostRequest(Path = "update-stories-command")]
public sealed class UpdateStoriesRequest : ICommand<UpdateStoriesRequest, UpdateStoriesResponse>
{
    public int Amount { get; set; }
    public List<Guid> StoryIds { get; set; } = [];
}
