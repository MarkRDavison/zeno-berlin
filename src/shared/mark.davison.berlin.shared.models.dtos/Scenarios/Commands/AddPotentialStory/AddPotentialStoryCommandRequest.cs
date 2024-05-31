namespace mark.davison.berlin.shared.models.dtos.Scenarios.Commands.AddPotentialStory;

[PostRequest(Path = "add-potential-story-command")]
public sealed class AddPotentialStoryCommandRequest : ICommand<AddPotentialStoryCommandRequest, AddPotentialStoryCommandResponse>
{
    public string StoryAddress { get; set; } = string.Empty;
    public Guid? SiteId { get; set; }
}
