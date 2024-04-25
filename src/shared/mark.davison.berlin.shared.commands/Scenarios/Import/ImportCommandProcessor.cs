namespace mark.davison.berlin.shared.commands.Scenarios.Import;

public class ImportCommandProcessor : ICommandProcessor<ImportCommandRequest, ImportCommandResponse>
{
    private readonly IRepository _repository;
    private readonly ICommandHandler<AddStoryCommandRequest, AddStoryCommandResponse> _addStoryCommandHandler;

    public ImportCommandProcessor(
        IRepository repository,
        ICommandHandler<AddStoryCommandRequest, AddStoryCommandResponse> addStoryCommandHandler)
    {
        _repository = repository;
        _addStoryCommandHandler = addStoryCommandHandler;
    }

    public async Task<ImportCommandResponse> ProcessAsync(ImportCommandRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var response = new ImportCommandResponse();

        if (!request.Data.Stories.Any())
        {
            response.Warnings.Add(ValidationMessages.FormatMessageParameters(
                ValidationMessages.NO_ITEMS,
                nameof(Story)));
        }

        await using (_repository.BeginTransaction())
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

                response.Imported++;

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

            await _repository.UpsertEntitiesAsync(updates, cancellationToken);

            await _repository.CommitTransactionAsync();
        }

        return response;
    }
}
