namespace mark.davison.berlin.shared.models.dto.Scenarios.Commands.Export;

public sealed class ExportCommandResponse : Response<SerialisedtDataDto>, IJobResponse
{
    public Guid? JobId { get; set; }
    public string JobStatus { get; set; } = string.Empty;
}
