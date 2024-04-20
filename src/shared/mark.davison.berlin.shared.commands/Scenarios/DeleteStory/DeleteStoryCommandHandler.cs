namespace mark.davison.berlin.shared.commands.Scenarios.DeleteStory;

public class DeleteStoryCommandHandler : ValidateAndProcessCommandHandler<DeleteStoryCommandRequest, DeleteStoryCommandResponse>
{
    public DeleteStoryCommandHandler(ICommandProcessor<DeleteStoryCommandRequest, DeleteStoryCommandResponse> processor) : base(processor)
    {
    }
}
