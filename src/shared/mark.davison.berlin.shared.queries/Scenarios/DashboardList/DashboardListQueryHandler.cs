﻿namespace mark.davison.berlin.shared.queries.Scenarios.DashboardList;

public class DashboardListQueryHandler : IQueryHandler<DashboardListQueryRequest, DashboardListQueryResponse>
{
    public IReadonlyRepository _repository;

    public DashboardListQueryHandler(IReadonlyRepository repository)
    {
        _repository = repository;
    }

    public async Task<DashboardListQueryResponse> Handle(DashboardListQueryRequest query, ICurrentUserContext currentUserContext, CancellationToken cancellation)
    {
        await using (_repository.BeginTransaction())
        {
            var response = new DashboardListQueryResponse
            {
                Value = []
            };

            var storyUpdates = await _repository.QueryEntities<StoryUpdate>()
                .Include(_ => _.Story)
                .ThenInclude(_ => _!.StoryFandomLinks)
                .ThenInclude(_ => _.Fandom!)
                .ThenInclude(_ => _!.ParentFandom!)
                .Where(_ => _.UserId == currentUserContext.CurrentUser.Id && _.Story!.Favourite)
                .GroupBy(_ => _.StoryId)
                .Select(_ => new
                {
                    StoryId = _.Key,
                    Update = _.OrderByDescending(u => u.LastModified).First()
                })
                .ToListAsync();

            foreach (var row in storyUpdates)
            {
                if (row.Update.Story == null) { continue; }
                var dto = new DashboardTileDto
                {
                    StoryId = row.StoryId,
                    Name = row.Update.Story.Name,
                    CurrentChapters = row.Update.Story.CurrentChapters,
                    TotalChapters = row.Update.Story.TotalChapters,
                    Complete = row.Update.Story.Complete,
                    Favourite = row.Update.Story.Favourite,
                    LastChecked = row.Update.Story.LastChecked,
                    LastAuthored = row.Update.LastAuthored,
                    Fandoms = row.Update.Story.StoryFandomLinks.Select(_ => _.FandomId).Distinct().ToList()
                };
                response.Value.Add(dto);
            }

            return response;
        }
    }
}