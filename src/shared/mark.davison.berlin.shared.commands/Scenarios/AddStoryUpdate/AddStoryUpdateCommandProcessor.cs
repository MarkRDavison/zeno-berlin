namespace mark.davison.berlin.shared.commands.Scenarios.AddStoryUpdate;

public sealed class AddStoryUpdateCommandProcessor : ICommandProcessor<AddStoryUpdateCommandRequest, AddStoryUpdateCommandResponse>
{
    private readonly IDbContext<BerlinDbContext> _dbContext;
    private readonly IDateService _dateService;

    public AddStoryUpdateCommandProcessor(
        IDbContext<BerlinDbContext> dbContext,
        IDateService dateService)
    {
        _dbContext = dbContext;
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

        await _dbContext.AddAsync(update, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

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
