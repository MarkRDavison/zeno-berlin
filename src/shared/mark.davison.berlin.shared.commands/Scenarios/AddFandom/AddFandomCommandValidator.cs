namespace mark.davison.berlin.shared.commands.Scenarios.AddFandom;

public sealed class AddFandomCommandValidator : ICommandValidator<AddFandomCommandRequest, AddFandomCommandResponse>
{
    private readonly IDbContext<BerlinDbContext> _dbContext;

    public AddFandomCommandValidator(IDbContext<BerlinDbContext> dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AddFandomCommandResponse> ValidateAsync(AddFandomCommandRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var duplicate = await _dbContext.Set<Fandom>()
            .AsNoTracking()
            .AnyAsync(
                _ =>
                    _.ExternalName == request.ExternalName &&
                    _.IsUserSpecified == request.IsUserSpecified,
                cancellationToken);

        if (duplicate)
        {
            return ValidationMessages.CreateErrorResponse<AddFandomCommandResponse>(
                ValidationMessages.DUPLICATE_ENTITY,
                nameof(Fandom),
                nameof(Fandom.ExternalName));
        }

        if (request.ParentFandomId is Guid parentId)
        {
            var parentExists = await _dbContext.Set<Fandom>()
                .AsNoTracking()
                .AnyAsync(
                    _ => _.Id == parentId,
                    cancellationToken);

            if (!parentExists)
            {
                return ValidationMessages.CreateErrorResponse<AddFandomCommandResponse>(
                    ValidationMessages.FAILED_TO_FIND_ENTITY,
                    nameof(Fandom),
                    nameof(Fandom.ParentFandomId),
                    parentId.ToString());
            }
        }

        return new AddFandomCommandResponse();
    }
}
