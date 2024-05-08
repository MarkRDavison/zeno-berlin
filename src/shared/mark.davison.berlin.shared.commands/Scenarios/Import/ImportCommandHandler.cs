namespace mark.davison.berlin.shared.commands.Scenarios.Import;

public class ImportCommandHandler : ValidateAndProcessJobCommandHandler<ImportCommandRequest, ImportCommandResponse, ImportSummary>
{
    public ImportCommandHandler(
        ICommandProcessor<ImportCommandRequest, ImportCommandResponse> processor,
        ICommandValidator<ImportCommandRequest, ImportCommandResponse> validator,
        IRepository repository,
        IDistributedPubSub distributedPubSub
    ) : base(
        processor,
        validator,
        repository,
        distributedPubSub)
    {
    }

    protected override string NotificationKey => "jobeventkeyberlin_DEV".ToUpperInvariant(); // TODO: From config/a constant
}
