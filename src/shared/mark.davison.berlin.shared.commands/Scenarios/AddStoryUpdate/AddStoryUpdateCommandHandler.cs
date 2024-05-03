namespace mark.davison.berlin.shared.commands.Scenarios.AddStoryUpdate;

public class AddStoryUpdateCommandHandler : ValidateAndProcessCommandHandler<AddStoryUpdateCommandRequest, AddStoryUpdateCommandResponse>
{
    public AddStoryUpdateCommandHandler(
        ICommandProcessor<AddStoryUpdateCommandRequest, AddStoryUpdateCommandResponse> processor,
        ICommandValidator<AddStoryUpdateCommandRequest, AddStoryUpdateCommandResponse> validator
    ) : base(
        processor,
        validator)
    {
    }
}
