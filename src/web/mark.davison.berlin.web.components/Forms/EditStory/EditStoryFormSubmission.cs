namespace mark.davison.berlin.web.components.Forms.EditStory;

public class EditStoryFormSubmission : IFormSubmission<EditStoryFormViewModel>
{
    private readonly IClientHttpRepository _repository;

    public EditStoryFormSubmission(IClientHttpRepository repository)
    {
        _repository = repository;
    }

    public async Task<Response> Primary(EditStoryFormViewModel formViewModel)
    {
        var commandRequest = new EditStoryCommandRequest
        {
            StoryId = formViewModel.StoryId,
            Changes =
            [
                new DiscriminatedPropertyChangeset
                {
                    Name = nameof(StoryDto.UpdateTypeId),
                    PropertyType = typeof(Guid).FullName!,
                    Value = formViewModel.UpdateTypeId
                },
                new DiscriminatedPropertyChangeset
                {
                    Name = nameof(StoryDto.ConsumedChapters),
                    PropertyType = typeof(int?).FullName!,
                    Value = formViewModel.ConsumedChapters
                }
            ]
        };

        return await _repository.Post<EditStoryCommandResponse, EditStoryCommandRequest>(commandRequest, CancellationToken.None);
    }
}
