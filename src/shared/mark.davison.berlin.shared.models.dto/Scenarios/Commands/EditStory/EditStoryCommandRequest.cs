namespace mark.davison.berlin.shared.models.dto.Scenarios.Commands.EditStory;

[PostRequest(Path = "edit-story-command")]
public sealed class EditStoryCommandRequest : ICommand<EditStoryCommandRequest, EditStoryCommandResponse>
{
    public Guid StoryId { get; set; }
    public List<DiscriminatedPropertyChangeset> Changes { get; set; } = [];
}
