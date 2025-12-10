namespace mark.davison.berlin.api.commands.Scenarios.Import;

public sealed class ImportCommandHandler : ValidateAndProcessJobCommandHandler<ImportCommandRequest, ImportCommandResponse, ImportSummary>
{
    public ImportCommandHandler(
        ICommandProcessor<ImportCommandRequest, ImportCommandResponse> processor,
        ICommandValidator<ImportCommandRequest, ImportCommandResponse> validator,
        IDbContext<BerlinDbContext> dbContext,
        IDistributedPubSub distributedPubSub,
        IOptions<JobAppSettings> jobOptions
    ) : base(
        processor,
        validator,
        dbContext,
        distributedPubSub,
        jobOptions)
    {
    }
}
