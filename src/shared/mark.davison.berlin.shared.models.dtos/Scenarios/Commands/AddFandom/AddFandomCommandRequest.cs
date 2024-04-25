namespace mark.davison.berlin.shared.models.dtos.Scenarios.Commands.AddFandom;

[PostRequest(Path = "add-fandom-command")]
public class AddFandomCommandRequest : ICommand<AddFandomCommandRequest, AddFandomCommandResponse>
{
    public bool IsHidden { get; set; }
    public bool IsUserSpecified { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ExternalName { get; set; } = string.Empty;
    public Guid? ParentFandomId { get; set; }
}
