namespace mark.davison.berlin.api.commands.Scenarios.Export;

public sealed class ExportCommandHandler : ValidateAndProcessJobCommandHandler<ExportCommandRequest, ExportCommandResponse, SerialisedtDataDto>
{
    public ExportCommandHandler(
        ICommandProcessor<ExportCommandRequest, ExportCommandResponse> processor,
        IDbContext<BerlinDbContext> dbContext,
        IDistributedPubSub distributedPubSub,
        IOptions<JobSettings> jobOptions
    ) : base(
        processor,
        dbContext,
        distributedPubSub,
        jobOptions)
    {
    }
}
