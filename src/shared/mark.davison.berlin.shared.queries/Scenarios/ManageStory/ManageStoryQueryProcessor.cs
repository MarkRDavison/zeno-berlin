namespace mark.davison.berlin.shared.queries.Scenarios.ManageStory;

public sealed class ManageStoryQueryProcessor : IQueryProcessor<ManageStoryQueryRequest, ManageStoryQueryResponse>
{
    private readonly IReadonlyRepository _repository;

    public ManageStoryQueryProcessor(IReadonlyRepository repository)
    {
        _repository = repository;
    }

    // TODO: two part query? get header info then get list of updates???
    public async Task<ManageStoryQueryResponse> ProcessAsync(ManageStoryQueryRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        await using (_repository.BeginTransaction())
        {

            var story = await _repository.QueryEntities<Story>()
                .Include(_ => _!.StoryFandomLinks)
                .Include(_ => _!.StoryAuthorLinks)
                .Where(_ => _.Id == request.StoryId && _.UserId == currentUserContext.CurrentUser.Id)
                .FirstOrDefaultAsync();

            if (story == null)
            {
                return ValidationMessages.CreateErrorResponse<ManageStoryQueryResponse>(
                    ValidationMessages.FAILED_TO_FIND_ENTITY,
                    nameof(Story),
                    request.StoryId.ToString());
            }

            var updates = await _repository.QueryEntities<StoryUpdate>()
                .Where(_ => _.StoryId == request.StoryId && _.UserId == currentUserContext.CurrentUser.Id)
                .ToListAsync();

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
}
