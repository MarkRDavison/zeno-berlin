
namespace mark.davison.berlin.shared.models.dto.Scenarios.Commands.Export;

[PostRequest(Path = "export-command")]
public sealed class ExportCommandRequest : ICommand<ExportCommandRequest, ExportCommandResponse>, IJobRequest
{
    public bool TriggerImmediateJob { get; set; }
    public bool UseJob { get; set; }
    public Guid? JobId { get; set; }
}
