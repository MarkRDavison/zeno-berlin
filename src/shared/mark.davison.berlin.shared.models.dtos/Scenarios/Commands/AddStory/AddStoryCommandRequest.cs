namespace mark.davison.berlin.shared.models.dtos.Scenarios.Commands.AddStory;

[PostRequest(Path = "add-story-command")]
public class AddStoryCommandRequest : ICommand<AddStoryCommandRequest, AddStoryCommandResponse>
{
    public string StoryAddress { get; set; } = string.Empty;
    public Guid? SiteId { get; set; }
}
