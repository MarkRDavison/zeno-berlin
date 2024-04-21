namespace mark.davison.berlin.shared.models.dtos.Scenarios.Commands.Export;

[PostRequest(Path = "export-command")]
public class ExportCommandRequest : ICommand<ExportCommandRequest, ExportCommandResponse>
{

}
