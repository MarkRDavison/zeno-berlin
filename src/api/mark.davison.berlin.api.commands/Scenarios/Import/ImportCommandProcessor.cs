namespace mark.davison.berlin.api.commands.Scenarios.Import;

public sealed class ImportCommandProcessor : ICommandProcessor<ImportCommandRequest, ImportCommandResponse>
{
    private readonly IDbContext<BerlinDbContext> _dbContext;
    private readonly ICommandHandler<AddStoryCommandRequest, AddStoryCommandResponse> _addStoryCommandHandler;
    private readonly IDateService _dateService;

    public ImportCommandProcessor(
        IDbContext<BerlinDbContext> dbContext,
        ICommandHandler<AddStoryCommandRequest, AddStoryCommandResponse> addStoryCommandHandler,
        IDateService dateService)
    {
        _dbContext = dbContext;
        _addStoryCommandHandler = addStoryCommandHandler;
        _dateService = dateService;
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
                    SuppressUpdateCreation = story.Updates.Any(),
                    AddWithoutRemoteData = request.AddWithoutRemoteData
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

                var importedStory = await _dbContext.GetByIdAsync<Story>(addStoryResponse.Value.Id, cancellationToken);

                if (importedStory is null)
                {
                    continue;
                }

                importedStory.Name = story.Name;
                importedStory.ConsumedChapters = story.ConsumedChapters;
                importedStory.CurrentChapters = story.CurrentChapters;
                importedStory.TotalChapters = story.TotalChapters;

                foreach (var update in story.Updates)
                {
                    var newUpdate = new StoryUpdate
                    {
                        Id = Guid.NewGuid(),
                        StoryId = importedStory.Id,
                        UserId = currentUserContext.UserId,
                        Complete = update.Complete,
                        ChapterTitle = update.ChapterTitle,
                        CurrentChapters = update.CurrentChapters,
                        TotalChapters = update.TotalChapters,
                        LastAuthored = update.LastAuthored,
                        Created = _dateService.Now,
                        LastModified = _dateService.Now,
                        ChapterAddress = update.ChapterAddress
                    };

                    updates.Add(newUpdate);
                }
            }

            await _dbContext.Set<StoryUpdate>().AddRangeAsync(updates, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitTransactionAsync(cancellationToken);
        }

        response.Warnings = [.. response.Warnings.Distinct()];

        return response;
    }
}
