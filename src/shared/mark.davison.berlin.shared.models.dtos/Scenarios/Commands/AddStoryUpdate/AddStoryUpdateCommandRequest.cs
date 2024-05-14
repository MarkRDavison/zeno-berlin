namespace mark.davison.berlin.shared.models.dtos.Scenarios.Commands.AddStoryUpdate;

[PostRequest(Path = "add-story-update-command")]
public sealed class AddStoryUpdateCommandRequest : ICommand<AddStoryUpdateCommandRequest, AddStoryUpdateCommandResponse>
{
    public Guid StoryId { get; set; }
    public int CurrentChapters { get; set; }
    public int? TotalChapters { get; set; }
    public bool Complete { get; set; }
    public DateOnly UpdateDate { get; set; }
}
