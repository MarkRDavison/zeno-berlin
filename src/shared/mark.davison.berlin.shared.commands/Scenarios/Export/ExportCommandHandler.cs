namespace mark.davison.berlin.shared.commands.Scenarios.Export;

public class ExportCommandHandler : ValidateAndProcessCommandHandler<ExportCommandRequest, ExportCommandResponse>
{
    public ExportCommandHandler(
        ICommandProcessor<ExportCommandRequest, ExportCommandResponse> processor
    ) : base(processor)
    {
    }
}
