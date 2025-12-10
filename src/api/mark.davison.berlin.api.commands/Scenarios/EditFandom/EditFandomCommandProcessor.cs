namespace mark.davison.berlin.api.commands.Scenarios.EditFandom;

public sealed class EditFandomCommandProcessor : ICommandProcessor<EditFandomCommandRequest, EditFandomCommandResponse>
{
    private readonly IDbContext<BerlinDbContext> _dbContext;

    public EditFandomCommandProcessor(IDbContext<BerlinDbContext> dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<EditFandomCommandResponse> ProcessAsync(EditFandomCommandRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var existing = await _dbContext
            .Set<Fandom>()
            .Where(_ => _.Id == request.FandomId && _.UserId == currentUserContext.UserId)
            .FirstOrDefaultAsync();

        if (existing == null)
        {
            return ValidationMessages.CreateErrorResponse<EditFandomCommandResponse>(
                ValidationMessages.FAILED_TO_FIND_ENTITY,
                nameof(Fandom),
                request.FandomId.ToString());
        }

        var type = typeof(Fandom);

        foreach (var cs in request.Changes)
        {
            var prop = type.GetProperty(cs.Name);

            if (prop == null || prop.PropertyType.FullName != cs.PropertyType) { continue; }

            prop.SetValue(existing, cs.Value);
        }

        _dbContext.Set<Fandom>().Update(existing);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new()
        {
            Value = new FandomDto // TODO: Helper
            {
                FandomId = existing.Id,
                ParentFandomId = existing.ParentFandomId,
                Name = existing.Name,
                ExternalName = existing.ExternalName,
                IsHidden = existing.IsHidden,
                IsUserSpecified = existing.IsUserSpecified
            }
        };
    }
}
