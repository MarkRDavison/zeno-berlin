namespace mark.davison.berlin.shared.commands.Scenarios.AddStoryUpdate;

public sealed class AddStoryUpdateCommandValidator : ICommandValidator<AddStoryUpdateCommandRequest, AddStoryUpdateCommandResponse>
{
    private readonly IValidationContext _validationContext;

    public AddStoryUpdateCommandValidator(IValidationContext validationContext)
    {
        _validationContext = validationContext;
    }

    public async Task<AddStoryUpdateCommandResponse> ValidateAsync(AddStoryUpdateCommandRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var story = await _validationContext.GetById<Story>(request.StoryId, cancellationToken);

        if (story == null)
        {
            return ValidationMessages.CreateErrorResponse<AddStoryUpdateCommandResponse>(
                ValidationMessages.FAILED_TO_FIND_ENTITY,
                nameof(Story),
                request.StoryId.ToString());
        }

        if (story.UserId != currentUserContext.CurrentUser.Id)
        {
            return ValidationMessages.CreateErrorResponse<AddStoryUpdateCommandResponse>(
                ValidationMessages.OWNERSHIP_MISMATCH);
        }

        var update = await _validationContext.GetByProperty<StoryUpdate>(
            _ => _.StoryId == request.StoryId && _.CurrentChapters == request.CurrentChapters,
            nameof(StoryUpdate) + "_" + nameof(StoryUpdate.CurrentChapters),
            cancellationToken);

        if (update != null)
        {
            return ValidationMessages.CreateErrorResponse<AddStoryUpdateCommandResponse>(
                ValidationMessages.DUPLICATE_ENTITY,
                nameof(StoryUpdate));
        }

        return new AddStoryUpdateCommandResponse();
    }
}
