namespace mark.davison.berlin.shared.commands.Scenarios.EditFandom;

public sealed class EditFandomCommandHandler : ValidateAndProcessCommandHandler<EditFandomCommandRequest, EditFandomCommandResponse>
{
    public EditFandomCommandHandler(
        ICommandProcessor<EditFandomCommandRequest, EditFandomCommandResponse> processor
    ) : base(processor)
    {
    }
}
