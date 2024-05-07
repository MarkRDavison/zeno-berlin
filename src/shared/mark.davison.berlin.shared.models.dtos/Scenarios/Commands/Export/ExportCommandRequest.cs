
namespace mark.davison.berlin.shared.models.dtos.Scenarios.Commands.Export;

[PostRequest(Path = "export-command")]
public class ExportCommandRequest : ICommand<ExportCommandRequest, ExportCommandResponse>, IJobRequest
{
    public bool TriggerImmediateJob { get; set; }
    public bool UseJob { get; set; }
    public Guid? JobId { get; set; }
}
