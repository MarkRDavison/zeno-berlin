namespace mark.davison.berlin.web.components.Forms.AddPotentialStory;

public sealed class AddPotentialStoryFormViewModel : IFormViewModel
{
    public string StoryAddress { get; set; } = string.Empty;

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
