namespace mark.davison.berlin.api.commands.Scenarios.AddFandom;

public sealed class AddFandomCommandProcessor : ICommandProcessor<AddFandomCommandRequest, AddFandomCommandResponse>
{
    private readonly IDbContext<BerlinDbContext> _dbContext;
    private readonly IDateService _dateService;

    public AddFandomCommandProcessor(
        IDbContext<BerlinDbContext> dbContext,
        IDateService dateService)
    {
        _dbContext = dbContext;
        _dateService = dateService;
    }

    public async Task<AddFandomCommandResponse> ProcessAsync(AddFandomCommandRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var fandom = new Fandom
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            ExternalName = request.ExternalName,
            UserId = currentUserContext.UserId,
            IsHidden = request.IsHidden,
            IsUserSpecified = request.IsUserSpecified,
            ParentFandomId = request.ParentFandomId,
            Created = _dateService.Now,
            LastModified = _dateService.Now // TODO: Interceptor or something to set created/lastmodified
        };

        var fandomResult = await _dbContext.AddAsync(fandom, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var newFandom = fandomResult?.Entity;

        if (newFandom == null)
        {
            return ValidationMessages.CreateErrorResponse<AddFandomCommandResponse>(
                ValidationMessages.ERROR_SAVING,
                nameof(Fandom));
        }

        return new()
        {
            Value = new FandomDto // TODO: Helper
            {
                FandomId = newFandom.Id,
                ParentFandomId = newFandom.ParentFandomId,
                Name = newFandom.Name,
                ExternalName = newFandom.ExternalName,
                IsHidden = newFandom.IsHidden,
                IsUserSpecified = newFandom.IsUserSpecified
            }
        };
    }
}
