﻿namespace mark.davison.berlin.shared.queries.Scenarios.StartupQuery;

public class StartupQueryHandler : IQueryHandler<StartupQueryRequest, StartupQueryResponse>
{
    private readonly IReadonlyRepository _repository;

    public StartupQueryHandler(IReadonlyRepository repository)
    {
        _repository = repository;
    }

    public async Task<StartupQueryResponse> Handle(StartupQueryRequest query, ICurrentUserContext currentUserContext, CancellationToken cancellation)
    {
        await using (_repository.BeginTransaction())
        {
            var options = await _repository.GetEntityAsync<UserOptions>(
                _ => _.UserId == currentUserContext.CurrentUser.Id,
                cancellation);

            if (options == null)
            {
                return new StartupQueryResponse
                {
                    Errors = [ValidationMessages.MissingUserOptions]
                };
            }

            return new StartupQueryResponse
            {
                Admin = options.IsAdmin
            };
        }
    }
}
