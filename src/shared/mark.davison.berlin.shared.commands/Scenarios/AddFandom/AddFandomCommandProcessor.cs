namespace mark.davison.berlin.shared.commands.Scenarios.AddFandom;

public sealed class AddFandomCommandProcessor : ICommandProcessor<AddFandomCommandRequest, AddFandomCommandResponse>
{
    private readonly IRepository _repository;
    private readonly IDateService _dateService;

    public AddFandomCommandProcessor(
        IRepository repository,
        IDateService dateService)
    {
        _repository = repository;
        _dateService = dateService;
    }

    public async Task<AddFandomCommandResponse> ProcessAsync(AddFandomCommandRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        await using (_repository.BeginTransaction())
        {
            var fandom = new Fandom
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                ExternalName = request.ExternalName,
                UserId = currentUserContext.CurrentUser.Id,
                IsHidden = request.IsHidden,
                IsUserSpecified = request.IsUserSpecified,
                ParentFandomId = request.ParentFandomId,
                Created = _dateService.Now,
                LastModified = _dateService.Now // TODO: Interceptor or something to set created/lastmodified
            };

            var newFandom = await _repository.UpsertEntityAsync(fandom, cancellationToken);

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
}
