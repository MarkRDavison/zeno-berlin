namespace mark.davison.berlin.shared.models.dto.Scenarios.Commands.DeleteStory;

[PostRequest(Path = "delete-story-command")]
public sealed class DeleteStoryCommandRequest : ICommand<DeleteStoryCommandRequest, DeleteStoryCommandResponse>
{
    public Guid StoryId { get; set; }
}
