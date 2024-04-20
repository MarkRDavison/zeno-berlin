namespace mark.davison.berlin.shared.queries.Scenarios.DashboardQuery;

public class DashboardQueryHandler : IQueryHandler<DashboardQueryRequest, DashboardQueryResponse>
{
    private readonly IReadonlyRepository _repository;

    public DashboardQueryHandler(IReadonlyRepository repository)
    {
        _repository = repository;
    }

    public async Task<DashboardQueryResponse> Handle(DashboardQueryRequest query, ICurrentUserContext currentUserContext, CancellationToken cancellation)
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
                    dto.LastModified = _.Update.LastModified;
                    return dto;
                })
                .ToList());

            return new DashboardQueryResponse
            {
                Value = dashboardData
            };
        }
    }
}
