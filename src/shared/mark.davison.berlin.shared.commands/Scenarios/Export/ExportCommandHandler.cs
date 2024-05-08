namespace mark.davison.berlin.shared.commands.Scenarios.Export;

public class ExportCommandHandler : ValidateAndProcessJobCommandHandler<ExportCommandRequest, ExportCommandResponse, SerialisedtDataDto>
{
    public ExportCommandHandler(
        ICommandProcessor<ExportCommandRequest, ExportCommandResponse> processor,
        IRepository repository,
        IDistributedPubSub distributedPubSub
    ) : base(
        processor,
        repository,
        distributedPubSub)
    {
    }

    protected override string NotificationKey => "jobeventkeyberlin_DEV".ToUpperInvariant(); // TODO: From config/a constant
}
