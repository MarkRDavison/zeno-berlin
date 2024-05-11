namespace mark.davison.berlin.shared.commands.Scenarios.Import;

public class ImportCommandHandler : ValidateAndProcessJobCommandHandler<ImportCommandRequest, ImportCommandResponse, ImportSummary>
{
    public ImportCommandHandler(
        ICommandProcessor<ImportCommandRequest, ImportCommandResponse> processor,
        ICommandValidator<ImportCommandRequest, ImportCommandResponse> validator,
        IRepository repository,
        IDistributedPubSub distributedPubSub,
        IOptions<JobAppSettings> jobOptions
    ) : base(
        processor,
        validator,
        repository,
        distributedPubSub,
        jobOptions)
    {
    }
}
