namespace mark.davison.berlin.shared.models.dtos.Scenarios.Commands.Import;

public sealed class ImportCommandResponse : Response<ImportSummary>, IJobResponse
{
    public Guid? JobId { get; set; }
    public string JobStatus { get; set; } = string.Empty;
}
