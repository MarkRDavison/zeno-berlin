namespace mark.davison.berlin.web.components.Forms.AddStory;

public class AddStoryFormViewModel : IFormViewModel
{
    public string StoryAddress { get; set; } = string.Empty;
    public Guid? UpdateTypeId { get; set; }

    public List<UpdateTypeDto> UpdateTypes { get; set; } = [];

    public IEnumerable<DropdownItem> UpdateTypeItems => UpdateTypes.Select(_ => new DropdownItem { Id = _.Id, Name = _.Description });

    public bool Valid
    {
        get
        {
            if (string.IsNullOrEmpty(StoryAddress))
            {
                return false;
            }

            Uri? uriResult;

            bool result = Uri.TryCreate(StoryAddress, UriKind.Absolute, out uriResult);

            // TODO: Validate that the url matches one of the supported sites

            return result;
        }
    }
}
