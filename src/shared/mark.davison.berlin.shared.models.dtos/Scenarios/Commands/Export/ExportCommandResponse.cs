namespace mark.davison.berlin.shared.models.dtos.Scenarios.Commands.Export;

public class ExportCommandResponse : Response<SerialisedtDataDto>, IJobResponse
{
    public Guid? JobId { get; set; }
    public string JobStatus { get; set; } = string.Empty;
}
