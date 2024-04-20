namespace mark.davison.berlin.shared.queries.Scenarios.StoryList;

public class StoryListQueryHandler : IQueryHandler<StoryListQueryRequest, StoryListQueryResponse>
{
    private readonly IReadonlyRepository _repository;

    public StoryListQueryHandler(IReadonlyRepository repository)
    {
        _repository = repository;
    }

    public async Task<StoryListQueryResponse> Handle(StoryListQueryRequest query, ICurrentUserContext currentUserContext, CancellationToken cancellation)
    {
        await using (_repository.BeginTransaction())
        {
            var dashboardData = new DashboardDataDto();

            var storyUpdates = await _repository.QueryEntities<StoryUpdate>()
                .Include(_ => _.Story)
                .Where(_ => _.UserId == currentUserContext.CurrentUser.Id)
                .GroupBy(_ => _.StoryId)
                .Select(_ => new
                {
                    StoryId = _.Key,
                    Update = _.OrderByDescending(u => u.LastModified).First()
                })
                .ToListAsync();

            dashboardData.Stories = new(storyUpdates
                .Where(_ => _.Update.Story != null)
                .Select(_ =>
                {
                    var dto = _.Update.Story!.ToDto();
                    dto.LastModified = _.Update.UpdateDate;
                    return dto;
                })
                .ToList());

            return new StoryListQueryResponse
            {
                Value = dashboardData
            };
        }
    }
}
