namespace mark.davison.berlin.shared.commands.Scenarios.AddStoryUpdate;

public sealed class AddStoryUpdateCommandProcessor : ICommandProcessor<AddStoryUpdateCommandRequest, AddStoryUpdateCommandResponse>
{
    private readonly IRepository _repository;
    private readonly IDateService _dateService;

    public AddStoryUpdateCommandProcessor(
        IRepository repository,
        IDateService dateService)
    {
        _repository = repository;
        _dateService = dateService;
    }


    public async Task<AddStoryUpdateCommandResponse> ProcessAsync(AddStoryUpdateCommandRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var update = new StoryUpdate
        {
            Id = Guid.NewGuid(),
            StoryId = request.StoryId,
            CurrentChapters = request.CurrentChapters,
            TotalChapters = request.TotalChapters,
            Complete = request.Complete,
            Created = request.UpdateDate.ToDateTime(TimeOnly.MinValue),
            LastAuthored = request.UpdateDate,
            LastModified = _dateService.Now,
            UserId = currentUserContext.CurrentUser.Id
        };

        await using (_repository.BeginTransaction())
        {
            await _repository.UpsertEntityAsync(update, cancellationToken);
        }

        return new AddStoryUpdateCommandResponse
        {
            Value = new StoryUpdateDto
            {
                StoryId = request.StoryId,
                CurrentChapters = request.CurrentChapters,
                TotalChapters = request.TotalChapters,
                Complete = request.Complete,
                UpdateDate = request.UpdateDate
            }
        };
    }
}
