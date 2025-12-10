namespace mark.davison.berlin.api.commands.Scenarios.DeleteStory;

public sealed class DeleteStoryCommandProcessor : ICommandProcessor<DeleteStoryCommandRequest, DeleteStoryCommandResponse>
{
    private readonly IDbContext<BerlinDbContext> _dbContext;

    public DeleteStoryCommandProcessor(IDbContext<BerlinDbContext> dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DeleteStoryCommandResponse> ProcessAsync(DeleteStoryCommandRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var story = await _dbContext
            .Set<Story>()
            .Where(_ => _.Id == request.StoryId && _.UserId == currentUserContext.UserId)
            .FirstOrDefaultAsync();

        if (story == null)
        {
            return ValidationMessages.CreateErrorResponse<DeleteStoryCommandResponse>(ValidationMessages.INVALID_PROPERTY, nameof(DeleteStoryCommandRequest.StoryId));
        }

        var updates = await _dbContext.Set<StoryUpdate>()
            .Where(_ => _.StoryId == request.StoryId)
            .ToListAsync();

        _dbContext.Set<Story>().Remove(story);
        _dbContext.Set<StoryUpdate>().RemoveRange(updates);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new DeleteStoryCommandResponse
        {
            DeletedStoryId = story.Id,
            DeletedStoryUpdateIds = [.. updates.Select(_ => _.Id)]
        };
    }
}
