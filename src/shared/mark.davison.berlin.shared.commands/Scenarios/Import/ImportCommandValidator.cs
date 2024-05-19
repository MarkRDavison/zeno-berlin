namespace mark.davison.berlin.shared.commands.Scenarios.Import;

public sealed class ImportCommandValidator : ICommandValidator<ImportCommandRequest, ImportCommandResponse>
{
    private readonly IDbContext<BerlinDbContext> _dbContext;

    public ImportCommandValidator(
        IDbContext<BerlinDbContext> dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ImportCommandResponse> ValidateAsync(ImportCommandRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var addressList = await _dbContext
            .Set<Story>()
            .AsNoTracking()
            .Where(_ => _.UserId == currentUserContext.CurrentUser.Id)
            .Select(_ => _.Address)
            .ToListAsync(cancellationToken);

        var existingAddresses = addressList.ToHashSet();

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
