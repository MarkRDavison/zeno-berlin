
namespace mark.davison.berlin.shared.commands.Scenarios.Import;

public class ImportCommandHandler : ValidateAndProcessCommandHandler<ImportCommandRequest, ImportCommandResponse>
{
    public ImportCommandHandler(
        ICommandProcessor<ImportCommandRequest, ImportCommandResponse> processor,
        ICommandValidator<ImportCommandRequest, ImportCommandResponse> validator
    ) : base(processor, validator)
    {
    }
}
