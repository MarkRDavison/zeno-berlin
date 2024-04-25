namespace mark.davison.berlin.shared.commands.Scenarios.EditFandom;

public class EditFandomCommandProcessor : ICommandProcessor<EditFandomCommandRequest, EditFandomCommandResponse>
{
    private readonly IRepository _repository;

    public EditFandomCommandProcessor(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<EditFandomCommandResponse> ProcessAsync(EditFandomCommandRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        await using (_repository.BeginTransaction())
        {
            var existing = await _repository.QueryEntities<Fandom>()
                .Where(_ => _.Id == request.FandomId && _.UserId == currentUserContext.CurrentUser.Id)
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

            var updated = await _repository.UpsertEntityAsync(existing, cancellationToken);

            if (updated == null)
            {
                return ValidationMessages.CreateErrorResponse<EditFandomCommandResponse>(
                    ValidationMessages.ERROR_SAVING);
            }

            return new()
            {
                Value = new FandomDto // TODO: Helper
                {
                    FandomId = updated.Id,
                    ParentFandomId = updated.ParentFandomId,
                    Name = updated.Name,
                    ExternalName = updated.ExternalName,
                    IsHidden = updated.IsHidden,
                    IsUserSpecified = updated.IsUserSpecified
                }
            };
        }
    }
}
