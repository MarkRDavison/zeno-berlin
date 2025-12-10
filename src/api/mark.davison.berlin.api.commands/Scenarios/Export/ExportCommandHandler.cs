namespace mark.davison.berlin.api.commands.Scenarios.Export;

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
