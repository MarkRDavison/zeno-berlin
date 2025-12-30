namespace mark.davison.berlin.api.commands.Scenarios.EditStory;

public sealed class EditStoryCommandProcessor : ICommandProcessor<EditStoryCommandRequest, EditStoryCommandResponse>
{
    private readonly IDbContext<BerlinDbContext> _dbContext;

    public EditStoryCommandProcessor(IDbContext<BerlinDbContext> dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<EditStoryCommandResponse> ProcessAsync(EditStoryCommandRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var existing = await _dbContext
            .Set<Story>()
            .Include(_ => _.StoryFandomLinks)
            .Where(_ => _.Id == request.StoryId && _.UserId == currentUserContext.UserId)
            .FirstOrDefaultAsync();

        if (existing == null)
        {
            return ValidationMessages.CreateErrorResponse<EditStoryCommandResponse>(ValidationMessages.FAILED_TO_FIND_ENTITY, nameof(Story), request.StoryId.ToString());
        }

        var type = typeof(Story);

        foreach (var cs in request.Changes)
        {
            var prop = type.GetProperty(cs.Name);

            if (prop == null || prop.PropertyType.FullName != cs.PropertyType) { continue; }

            prop.SetValue(existing, cs.Value);
        }

        await _dbContext.UpsertEntityAsync(existing, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new() { Value = existing.ToDto() };
    }
}
