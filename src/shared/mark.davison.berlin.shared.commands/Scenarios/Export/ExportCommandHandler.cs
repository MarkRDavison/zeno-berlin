namespace mark.davison.berlin.shared.commands.Scenarios.Export;

public class ExportCommandHandler : ValidateAndProcessJobCommandHandler<ExportCommandRequest, ExportCommandResponse, SerialisedtDataDto>
{
    public ExportCommandHandler(
        ICommandProcessor<ExportCommandRequest, ExportCommandResponse> processor,
        IRepository repository,
        IDistributedPubSub distributedPubSub,
        IOptions<JobAppSettings> jobOptions
    ) : base(
        processor,
        repository,
        distributedPubSub,
        jobOptions)
    {
    }
}
