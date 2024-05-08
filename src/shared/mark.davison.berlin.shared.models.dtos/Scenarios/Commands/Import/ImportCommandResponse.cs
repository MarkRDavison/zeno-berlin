namespace mark.davison.berlin.shared.models.dtos.Scenarios.Commands.Import;

public class ImportCommandResponse : Response<ImportSummary>, IJobResponse
{
    public Guid? JobId { get; set; }
    public string JobStatus { get; set; } = string.Empty;
}
