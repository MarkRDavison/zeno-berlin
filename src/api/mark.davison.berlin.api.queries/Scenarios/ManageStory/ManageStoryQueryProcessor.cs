namespace mark.davison.berlin.api.queries.Scenarios.ManageStory;

public sealed class ManageStoryQueryProcessor : IQueryProcessor<ManageStoryQueryRequest, ManageStoryQueryResponse>
{
    private readonly IDbContext<BerlinDbContext> _dbContext;

    public ManageStoryQueryProcessor(IDbContext<BerlinDbContext> dbContext)
    {
        _dbContext = dbContext;
    }

    // TODO: two part query? get header info then get list of updates???
    public async Task<ManageStoryQueryResponse> ProcessAsync(ManageStoryQueryRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var story = await _dbContext
            .Set<Story>()
            .AsNoTracking()
            .Include(_ => _!.StoryFandomLinks)
            .Include(_ => _!.StoryAuthorLinks)
            .Where(_ => _.Id == request.StoryId && _.UserId == currentUserContext.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (story == null)
        {
            return ValidationMessages.CreateErrorResponse<ManageStoryQueryResponse>(
                ValidationMessages.FAILED_TO_FIND_ENTITY,
                nameof(Story),
                request.StoryId.ToString());
        }

        var updates = await _dbContext
            .Set<StoryUpdate>()
            .AsNoTracking()
            .Where(_ => _.StoryId == request.StoryId && _.UserId == currentUserContext.UserId)
            .OrderByDescending(_ => _.LastAuthored)
            .Take(20)
            .ToListAsync(cancellationToken);

        var response = new ManageStoryQueryResponse
        {
            Value = new StoryManageDto
            {
                StoryId = story.Id,
                Name = story.Name,
                Address = story.Address,
                CurrentChapters = story.CurrentChapters,
                TotalChapters = story.TotalChapters,
                ConsumedChapters = story.ConsumedChapters,
                Complete = story.Complete,
                Favourite = story.Favourite,
                UpdateTypeId = story.UpdateTypeId,
                LastChecked = story.LastChecked,
                LastAuthored = story.LastAuthored,
                FandomIds = [.. story.StoryFandomLinks.Select(_ => _.FandomId)],
                AuthorIds = [.. story.StoryAuthorLinks.Select(_ => _.AuthorId)],
                Updates = [..updates.Select(_ => new StoryManageUpdatesDto {
                    CurrentChapters = _.CurrentChapters,
                    TotalChapters = _.TotalChapters,
                    Complete = _.Complete,
                    ChapterAddress = _.ChapterAddress ?? string.Empty,
                    ChapterTitle = _.ChapterTitle ?? string.Empty,
                    LastAuthored = _.LastAuthored,
                    LastChecked = _.LastModified
                }).OrderByDescending(_ => _.LastAuthored)]
            }
        };

        return response;
    }
}
