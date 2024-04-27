﻿namespace mark.davison.berlin.shared.queries.Scenarios.FandomList;

public class FandomListQueryHandler : IQueryHandler<FandomListQueryRequest, FandomListQueryResponse>
{
    private readonly IReadonlyRepository _repository;

    public FandomListQueryHandler(IReadonlyRepository repository)
    {
        _repository = repository;
    }

    public async Task<FandomListQueryResponse> Handle(FandomListQueryRequest query, ICurrentUserContext currentUserContext, CancellationToken cancellation)
    {
        await using (_repository.BeginTransaction())
        {
            var fandoms = await _repository.QueryEntities<Fandom>()
                .Where(_ => _.UserId == currentUserContext.CurrentUser.Id)
                .Select(_ => new FandomDto // TODO: Helper
                {
                    FandomId = _.Id,
                    ParentFandomId = _.ParentFandomId,
                    Name = _.Name,
                    ExternalName = _.ExternalName,
                    IsHidden = _.IsHidden,
                    IsUserSpecified = _.IsUserSpecified
                })
                .ToListAsync();

            return new FandomListQueryResponse
            {
                Value = fandoms
            };
        }
    }
}