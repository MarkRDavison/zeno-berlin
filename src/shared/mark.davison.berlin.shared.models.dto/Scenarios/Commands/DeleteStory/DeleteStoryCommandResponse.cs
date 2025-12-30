namespace mark.davison.berlin.shared.models.dto.Scenarios.Commands.DeleteStory;

public sealed class DeleteStoryCommandResponse : Response
{
    public Guid DeletedStoryId { get; set; }
    public List<Guid> DeletedStoryUpdateIds { get; set; } = new();
}
