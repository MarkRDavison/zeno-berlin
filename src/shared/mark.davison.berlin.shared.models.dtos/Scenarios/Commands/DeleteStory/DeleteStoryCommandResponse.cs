namespace mark.davison.berlin.shared.models.dtos.Scenarios.Commands.DeleteStory;

public class DeleteStoryCommandResponse : Response
{
    public Guid DeletedStoryId { get; set; }
    public List<Guid> DeletedStoryUpdateIds { get; set; } = new();
}
