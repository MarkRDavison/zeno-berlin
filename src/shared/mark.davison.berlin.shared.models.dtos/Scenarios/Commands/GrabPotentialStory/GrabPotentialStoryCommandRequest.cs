namespace mark.davison.berlin.shared.models.dtos.Scenarios.Commands.GrabPotentialStory;

[PostRequest(Path = "grab-potential-story-command")]
public sealed class GrabPotentialStoryCommandRequest : ICommand<GrabPotentialStoryCommandRequest, GrabPotentialStoryCommandResponse>
{
    public Guid Id { get; set; }
}
