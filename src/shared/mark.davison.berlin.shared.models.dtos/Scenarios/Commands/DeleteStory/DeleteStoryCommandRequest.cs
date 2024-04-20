namespace mark.davison.berlin.shared.models.dtos.Scenarios.Commands.DeleteStory;

[PostRequest(Path = "delete-story-command")]
public class DeleteStoryCommandRequest : ICommand<DeleteStoryCommandRequest, DeleteStoryCommandResponse>
{
    public Guid StoryId { get; set; }
}
