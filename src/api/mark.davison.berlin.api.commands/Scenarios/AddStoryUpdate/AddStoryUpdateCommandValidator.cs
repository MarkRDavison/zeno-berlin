namespace mark.davison.berlin.api.commands.Scenarios.AddStoryUpdate;

public sealed class AddStoryUpdateCommandValidator : ICommandValidator<AddStoryUpdateCommandRequest, AddStoryUpdateCommandResponse>
{
    private readonly IDbContext<BerlinDbContext> _dbContext;

    public AddStoryUpdateCommandValidator(IDbContext<BerlinDbContext> dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AddStoryUpdateCommandResponse> ValidateAsync(AddStoryUpdateCommandRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var story = await _dbContext
            .Set<Story>()
            .AsNoTracking()
            .Where(_ => _.Id == request.StoryId)
            .FirstOrDefaultAsync(cancellationToken);

        if (story == null)
        {
            return ValidationMessages.CreateErrorResponse<AddStoryUpdateCommandResponse>(
                ValidationMessages.FAILED_TO_FIND_ENTITY,
                nameof(Story),
                request.StoryId.ToString());
        }

        if (story.UserId != currentUserContext.UserId)
        {
            return ValidationMessages.CreateErrorResponse<AddStoryUpdateCommandResponse>(
                ValidationMessages.OWNERSHIP_MISMATCH);
        }

        var update = await _dbContext
            .Set<StoryUpdate>()
            .AsNoTracking()
            .Where(_ => _.StoryId == request.StoryId && _.CurrentChapters == request.CurrentChapters)
            .FirstOrDefaultAsync(cancellationToken);

        if (update != null)
        {
            return ValidationMessages.CreateErrorResponse<AddStoryUpdateCommandResponse>(
                ValidationMessages.DUPLICATE_ENTITY,
                nameof(StoryUpdate));
        }

        return new AddStoryUpdateCommandResponse();
    }
}
