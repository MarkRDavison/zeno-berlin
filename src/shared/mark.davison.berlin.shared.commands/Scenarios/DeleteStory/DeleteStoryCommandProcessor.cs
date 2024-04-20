namespace mark.davison.berlin.shared.commands.Scenarios.DeleteStory;

public class DeleteStoryCommandProcessor : ICommandProcessor<DeleteStoryCommandRequest, DeleteStoryCommandResponse>
{
    private readonly IRepository _repository;

    public DeleteStoryCommandProcessor(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<DeleteStoryCommandResponse> ProcessAsync(DeleteStoryCommandRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        await using (_repository.BeginTransaction())
        {
            var story = await _repository.QueryEntities<Story>()
                .Where(_ => _.Id == request.StoryId && _.UserId == currentUserContext.CurrentUser.Id)
                .FirstOrDefaultAsync();

            if (story == null)
            {
                return ValidationMessages.CreateErrorResponse<DeleteStoryCommandResponse>(ValidationMessages.INVALID_PROPERTY, nameof(DeleteStoryCommandRequest.StoryId));
            }

            var updates = await _repository.QueryEntities<StoryUpdate>()
                .Where(_ => _.StoryId == request.StoryId)
                .ToListAsync();

            var deletedStory = await _repository.DeleteEntityAsync(story, cancellationToken);
            var deletedUpdates = await _repository.DeleteEntitiesAsync(updates, cancellationToken);

            await _repository.CommitTransactionAsync();

            if (deletedStory == null)
            {
                return ValidationMessages.CreateErrorResponse<DeleteStoryCommandResponse>(ValidationMessages.ERROR_DELETING);
            }

            return new DeleteStoryCommandResponse
            {
                DeletedStoryId = deletedStory.Id,
                DeletedStoryUpdateIds = [.. deletedUpdates.Select(_ => _.Id)]
            };
        }
    }
}
