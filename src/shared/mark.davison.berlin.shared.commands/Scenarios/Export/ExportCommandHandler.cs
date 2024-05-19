namespace mark.davison.berlin.shared.commands.Scenarios.Export;

public sealed class ExportCommandHandler : ValidateAndProcessJobCommandHandler<ExportCommandRequest, ExportCommandResponse, SerialisedtDataDto>
{
    public ExportCommandHandler(
        ICommandProcessor<ExportCommandRequest, ExportCommandResponse> processor,
        IDbContext<BerlinDbContext> dbContext,
        IDistributedPubSub distributedPubSub,
        IOptions<JobAppSettings> jobOptions
    ) : base(
        processor,
        dbContext,
        distributedPubSub,
        jobOptions)
    {
    }
}
