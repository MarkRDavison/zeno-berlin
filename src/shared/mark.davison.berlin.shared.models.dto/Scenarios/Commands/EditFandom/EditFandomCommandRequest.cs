namespace mark.davison.berlin.shared.models.dto.Scenarios.Commands.EditFandom;

[PostRequest(Path = "edit-fandom-command")]
public sealed class EditFandomCommandRequest : ICommand<EditFandomCommandRequest, EditFandomCommandResponse>
{
    public Guid FandomId { get; set; }
    public List<DiscriminatedPropertyChangeset> Changes { get; set; } = [];
}
