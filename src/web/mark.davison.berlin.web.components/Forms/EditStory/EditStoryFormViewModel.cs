namespace mark.davison.berlin.web.components.Forms.EditStory;

public sealed class EditStoryFormViewModel : IFormViewModel
{
    public Guid StoryId { get; set; }
    public int? ConsumedChapters { get; set; }
    public int CurrentChapters { get; set; }
    public Guid? UpdateTypeId { get; set; }
    public List<UpdateTypeDto> UpdateTypes { get; set; } = [];

    public IEnumerable<DropdownItem> UpdateTypeItems => UpdateTypes.Select(_ => new DropdownItem { Id = _.Id, Name = _.Description });


    public bool Valid => UpdateTypeId != null && UpdateTypeId != default;
}
