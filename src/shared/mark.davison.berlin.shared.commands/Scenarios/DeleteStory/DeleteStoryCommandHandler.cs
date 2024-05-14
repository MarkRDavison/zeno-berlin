namespace mark.davison.berlin.shared.commands.Scenarios.DeleteStory;

public sealed class DeleteStoryCommandHandler : ValidateAndProcessCommandHandler<DeleteStoryCommandRequest, DeleteStoryCommandResponse>
{
    public DeleteStoryCommandHandler(ICommandProcessor<DeleteStoryCommandRequest, DeleteStoryCommandResponse> processor) : base(processor)
    {
    }
}
