namespace mark.davison.berlin.shared.commands.Scenarios.Export;

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
            .Where(_ => _.UserId == currentUserContext.CurrentUser.Id)
            .ToListAsync();

        var storyUpdates = await _dbContext
            .Set<StoryUpdate>()
            .AsNoTracking()
            .Where(_ => _.UserId == currentUserContext.CurrentUser.Id)
            .ToListAsync();

        var exportData = new SerialisedtDataDto
        {
            Version = 1,
            Stories = [.. stories.Select(s => CreateSerialisedStoryDto(s, storyUpdates.Where(u => u.StoryId == s.Id)))]
        };

        response.Value = exportData;

        return response;
    }

    private SerialisedStoryDto CreateSerialisedStoryDto(Story story, IEnumerable<StoryUpdate> storyUpdates)
    {
        return new SerialisedStoryDto
        {
            StoryAddress = story.Address,
            Favourite = story.Favourite,
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
