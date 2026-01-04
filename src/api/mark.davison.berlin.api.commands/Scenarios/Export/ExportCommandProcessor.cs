namespace mark.davison.berlin.api.commands.Scenarios.Export;

public sealed class ExportCommandProcessor : ICommandProcessor<ExportCommandRequest, ExportCommandResponse>
{
    private readonly IDbContext<BerlinDbContext> _dbContext;

    public ExportCommandProcessor(
        IDbContext<BerlinDbContext> dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ExportCommandResponse> ProcessAsync(ExportCommandRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var response = new ExportCommandResponse();

        var stories = await _dbContext
            .Set<Story>()
            .AsNoTracking()
            .Where(_ => _.UserId == currentUserContext.UserId)
            .ToListAsync(cancellationToken);

        var storyUpdates = await _dbContext
            .Set<StoryUpdate>()
            .AsNoTracking()
            .Where(_ => _.UserId == currentUserContext.UserId)
            .ToListAsync(cancellationToken);

        var exportData = new SerialisedtDataDto
        {
            Version = 2,
            Stories = [.. stories.Select(s => CreateSerialisedStoryDto(s, storyUpdates.Where(u => u.StoryId == s.Id)))]
        };

        response.Value = exportData;

        return response;
    }

    private SerialisedStoryDto CreateSerialisedStoryDto(Story story, IEnumerable<StoryUpdate> storyUpdates)
    {
        return new SerialisedStoryDto
        {
            Name = story.Name,
            StoryAddress = story.Address,
            Favourite = story.Favourite,
            ConsumedChapters = story.ConsumedChapters,
            Updates = [.. storyUpdates.OrderByDescending(_ => _.LastAuthored).Select(CreateSerialisedStoryUpdateDto)]
        };
    }

    private SerialisedStoryUpdateDto CreateSerialisedStoryUpdateDto(StoryUpdate update)
    {
        return new SerialisedStoryUpdateDto
        {
            Complete = update.Complete,
            CurrentChapters = update.CurrentChapters,
            TotalChapters = update.TotalChapters,
            LastAuthored = update.LastAuthored
        };
    }
}
