namespace mark.davison.berlin.shared.commands.Scenarios.AddFandom;

public class AddFandomCommandValidator : ICommandValidator<AddFandomCommandRequest, AddFandomCommandResponse>
{
    private readonly IReadonlyRepository _repository;

    public AddFandomCommandValidator(IReadonlyRepository repository)
    {
        _repository = repository;
    }

    public async Task<AddFandomCommandResponse> ValidateAsync(AddFandomCommandRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        await using (_repository.BeginTransaction())
        {
            var duplicate = await _repository.QueryEntities<Fandom>()
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
                var parentExists = await _repository.QueryEntities<Fandom>()
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
}
