namespace mark.davison.berlin.shared.models.dtos.Scenarios.Commands.DeletePotentialStory;

[PostRequest(Path = "delete-potential-story-command")]
public sealed class DeletePotentialStoryCommandRequest : ICommand<DeletePotentialStoryCommandRequest, DeletePotentialStoryCommandResponse>
{
    public Guid Id { get; set; }
}
