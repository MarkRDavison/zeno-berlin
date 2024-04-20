namespace mark.davison.berlin.shared.commands.Scenarios.EditStory;

public class EditStoryCommandProcessor : ICommandProcessor<EditStoryCommandRequest, EditStoryCommandResponse>
{
    private readonly IRepository _repository;

    public EditStoryCommandProcessor(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<EditStoryCommandResponse> ProcessAsync(EditStoryCommandRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        await using (_repository.BeginTransaction())
        {
            var existing = await _repository.QueryEntities<Story>()
                .Where(_ => _.Id == request.StoryId && _.UserId == currentUserContext.CurrentUser.Id)
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

            var updated = await _repository.UpsertEntityAsync(existing, cancellationToken);

            if (updated == null)
            {
                return ValidationMessages.CreateErrorResponse<EditStoryCommandResponse>(ValidationMessages.ERROR_SAVING);
            }

            return new() { Value = updated.ToDto() }; // TODO: LastChecked, LastUpdated????
        }
    }
}
