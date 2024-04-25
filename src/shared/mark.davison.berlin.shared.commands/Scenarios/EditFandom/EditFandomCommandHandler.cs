namespace mark.davison.berlin.shared.commands.Scenarios.EditFandom;

public class EditFandomCommandHandler : ValidateAndProcessCommandHandler<EditFandomCommandRequest, EditFandomCommandResponse>
{
    public EditFandomCommandHandler(
        ICommandProcessor<EditFandomCommandRequest, EditFandomCommandResponse> processor
    ) : base(processor)
    {
    }
}
