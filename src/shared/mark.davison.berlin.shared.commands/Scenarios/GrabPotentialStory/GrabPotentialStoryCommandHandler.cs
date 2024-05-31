namespace mark.davison.berlin.shared.commands.Scenarios.GrabPotentialStory;

public sealed class GrabPotentialStoryCommandHandler : ValidateAndProcessCommandHandler<GrabPotentialStoryCommandRequest, GrabPotentialStoryCommandResponse>
{
    public GrabPotentialStoryCommandHandler(
        ICommandProcessor<GrabPotentialStoryCommandRequest, GrabPotentialStoryCommandResponse> processor
    ) : base(
        processor)
    {
    }
}
