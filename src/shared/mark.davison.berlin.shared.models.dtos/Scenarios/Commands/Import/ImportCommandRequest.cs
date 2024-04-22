namespace mark.davison.berlin.shared.models.dtos.Scenarios.Commands.Import;

[PostRequest(Path = "import-command")]
public class ImportCommandRequest : ICommand<ImportCommandRequest, ImportCommandResponse>
{
    public SerialisedtDataDto Data { get; set; } = new();
}
