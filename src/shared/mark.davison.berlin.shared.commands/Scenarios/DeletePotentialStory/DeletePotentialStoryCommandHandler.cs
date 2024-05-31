namespace mark.davison.berlin.shared.commands.Scenarios.DeletePotentialStory;

public sealed class DeletePotentialStoryCommandHandler : ValidateAndProcessCommandHandler<DeletePotentialStoryCommandRequest, DeletePotentialStoryCommandResponse>
{
    public DeletePotentialStoryCommandHandler(ICommandProcessor<DeletePotentialStoryCommandRequest, DeletePotentialStoryCommandResponse> processor) : base(processor)
    {
    }
}
