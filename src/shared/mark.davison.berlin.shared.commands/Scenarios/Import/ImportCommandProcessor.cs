namespace mark.davison.berlin.shared.commands.Scenarios.Import;

public sealed class ImportCommandProcessor : ICommandProcessor<ImportCommandRequest, ImportCommandResponse>
{
    private readonly IDbContext<BerlinDbContext> _dbContext;
    private readonly ICommandHandler<AddStoryCommandRequest, AddStoryCommandResponse> _addStoryCommandHandler;

    public ImportCommandProcessor(
        IDbContext<BerlinDbContext> dbContext,
        ICommandHandler<AddStoryCommandRequest, AddStoryCommandResponse> addStoryCommandHandler)
    {
        _dbContext = dbContext;
        _addStoryCommandHandler = addStoryCommandHandler;
    }

    public async Task<ImportCommandResponse> ProcessAsync(ImportCommandRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var response = new ImportCommandResponse
        {
            Value = new()
        };

        if (!request.Data.Stories.Any())
        {
            response.Warnings.Add(ValidationMessages.FormatMessageParameters(
                ValidationMessages.NO_ITEMS,
                nameof(Story)));
        }

        using (var transaction = await _dbContext.BeginTransactionAsync(cancellationToken))
        {
            var updates = new List<StoryUpdate>();

            foreach (var story in request.Data.Stories)
            {
                var addStoryRequest = new AddStoryCommandRequest
                {
                    StoryAddress = story.StoryAddress,
                    Favourite = story.Favourite,
                    SuppressUpdateCreation = story.Updates.Any()
                };

                var addStoryResponse = await _addStoryCommandHandler.Handle(addStoryRequest, currentUserContext, cancellationToken);

                response.Warnings.AddRange(addStoryResponse.Warnings);

                if (!addStoryResponse.SuccessWithValue)
                {
                    response.Errors.Add(ValidationMessages.FormatMessageParameters(
                        ValidationMessages.FAILED_TO_IMPORT,
                        nameof(Story),
                        story.StoryAddress,
                        string.Join('&', addStoryResponse.Errors)));

                    continue;
                }

                response.Value.Imported++;

                foreach (var update in story.Updates)
                {
                    var newUpdate = new StoryUpdate
                    {
                        Id = Guid.NewGuid(),
                        StoryId = addStoryResponse.Value.Id,
                        UserId = currentUserContext.CurrentUser.Id,
                        Complete = update.Complete,
                        CurrentChapters = update.CurrentChapters,
                        TotalChapters = update.TotalChapters,
                        LastAuthored = update.LastAuthored
                    };

                    updates.Add(newUpdate);
                }
            }

            await _dbContext.Set<StoryUpdate>().AddRangeAsync(updates, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitTransactionAsync(cancellationToken);
        }

        return response;
    }
}
