namespace mark.davison.berlin.shared.commands.Scenarios.EditStory;

public sealed class EditStoryCommandHandler : ValidateAndProcessCommandHandler<EditStoryCommandRequest, EditStoryCommandResponse>
{
    public EditStoryCommandHandler(ICommandProcessor<EditStoryCommandRequest, EditStoryCommandResponse> processor) : base(processor)
    {
    }
}
