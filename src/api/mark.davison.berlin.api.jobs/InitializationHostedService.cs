namespace mark.davison.berlin.api.jobs;

public class InitializationHostedService : GenericApplicationHealthStateHostedService
{
    private readonly IJobOrchestrationService _jobOrchestrationService;

    public InitializationHostedService(
        IHostApplicationLifetime hostApplicationLifetime,
        IApplicationHealthState applicationHealthState,
        IJobOrchestrationService jobOrchestrationService
    ) : base(
        hostApplicationLifetime,
        applicationHealthState)
    {
        _jobOrchestrationService = jobOrchestrationService;
    }

    protected override async Task AdditionalStartAsync(CancellationToken cancellationToken)
    {
        await _jobOrchestrationService.InitialiseJobMonitoring();

        await base.AdditionalStartAsync(cancellationToken);
    }
}
