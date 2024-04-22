namespace mark.davison.berlin.shared.commands.Scenarios.Import;

public class ImportCommandValidator : ICommandValidator<ImportCommandRequest, ImportCommandResponse>
{
    private readonly IValidationContext _validationContext;

    public ImportCommandValidator(
        IValidationContext validationContext)
    {
        _validationContext = validationContext;
    }

    public async Task<ImportCommandResponse> ValidateAsync(ImportCommandRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {

        var existingStories = await _validationContext.GetAllForUserId<Story>(currentUserContext.CurrentUser.Id, cancellationToken);

        var existingAddresses = existingStories.Select(_ => _.Address).ToHashSet();

        var response = new ImportCommandResponse();

        var duplicate = request.Data.Stories.Count;

        request.Data.Stories.RemoveAll(_ => existingAddresses.Contains(_.StoryAddress));

        duplicate -= request.Data.Stories.Count;

        if (duplicate > 0)
        {
            response.Warnings.Add(ValidationMessages.FormatMessageParameters(ValidationMessages.DUPLICATE_ENTITY, nameof(Story), duplicate.ToString()));
        }

        return response;
    }
}
