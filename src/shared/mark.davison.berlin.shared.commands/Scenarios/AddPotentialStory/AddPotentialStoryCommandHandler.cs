namespace mark.davison.berlin.shared.commands.Scenarios.AddPotentialStory;

public sealed class AddPotentialStoryCommandHandler : ValidateAndProcessCommandHandler<AddPotentialStoryCommandRequest, AddPotentialStoryCommandResponse>
{
    public AddPotentialStoryCommandHandler(
        ICommandProcessor<AddPotentialStoryCommandRequest, AddPotentialStoryCommandResponse> processor,
        ICommandValidator<AddPotentialStoryCommandRequest, AddPotentialStoryCommandResponse> validator
    ) : base(
        processor,
        validator)
    {
    }
}
