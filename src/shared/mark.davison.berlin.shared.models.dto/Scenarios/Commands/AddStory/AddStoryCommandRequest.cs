namespace mark.davison.berlin.shared.models.dto.Scenarios.Commands.AddStory;

[PostRequest(Path = "add-story-command")]
public sealed class AddStoryCommandRequest : ICommand<AddStoryCommandRequest, AddStoryCommandResponse>
{
    public string StoryAddress { get; set; } = string.Empty;
    public Guid? SiteId { get; set; }
    public Guid? UpdateTypeId { get; set; }
    public bool Favourite { get; set; }
    public bool SuppressUpdateCreation { get; set; }
}
