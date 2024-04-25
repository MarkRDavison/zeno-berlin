namespace mark.davison.berlin.shared.models.dtos.Scenarios.Commands.EditFandom;

[PostRequest(Path = "edit-fandom-command")]
public class EditFandomCommandRequest : ICommand<EditFandomCommandRequest, EditFandomCommandResponse>
{
    public Guid FandomId { get; set; }
    public List<DiscriminatedPropertyChangeset> Changes { get; set; } = [];
}
