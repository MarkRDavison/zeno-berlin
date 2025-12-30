namespace mark.davison.berlin.shared.models.dto.Scenarios.Commands.Import;

public sealed class ImportCommandResponse : Response<ImportSummary>, IJobResponse
{
    public Guid? JobId { get; set; }
    public string JobStatus { get; set; } = string.Empty;
}
