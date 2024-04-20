namespace mark.davison.berlin.shared.commands.Scenarios.AddStory;

public class AddStoryCommandProcessor : ICommandProcessor<AddStoryCommandRequest, AddStoryCommandResponse>
{
    private readonly IDateService _dateService;
    private readonly IValidationContext _validationContext;
    protected readonly IServiceProvider _serviceProvider;

    public AddStoryCommandProcessor(
        IDateService dateService,
        IValidationContext validationContext,
        IServiceProvider serviceProvider
    )
    {
        _dateService = dateService;
        _validationContext = validationContext;
        _serviceProvider = serviceProvider;
    }

    public async Task<AddStoryCommandResponse> ProcessAsync(AddStoryCommandRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var repository = _serviceProvider.GetRequiredService<IRepository>();
        await using (repository.BeginTransaction())
        {
            var site = await _validationContext.GetById<Site>(request.SiteId!.Value, cancellationToken);

            var infoProcessor = _serviceProvider.GetRequiredKeyedService<IStoryInfoProcessor>(site!.ShortName);

            var externalId = infoProcessor.ExtractExternalStoryId(request.StoryAddress);

            var info = await infoProcessor.ExtractStoryInfo(request.StoryAddress, cancellationToken);

            var story = new Story
            {
                Id = Guid.NewGuid(),
                Address = infoProcessor.GenerateBaseStoryAddress(request.StoryAddress),
                ExternalId = externalId,
                SiteId = site.Id,
                UserId = currentUserContext.CurrentUser.Id,
                Complete = info.IsCompleted,
                CurrentChapters = info.CurrentChapters,
                Name = info.Name,
                TotalChapters = info.TotalChapters,
                UpdateTypeId = UpdateTypeConstants.EachChapterId,
                LastChecked = _dateService.Now
            };

            var storyUpdate = new StoryUpdate
            {
                Id = Guid.NewGuid(),
                StoryId = story.Id,
                UserId = currentUserContext.CurrentUser.Id,
                Complete = info.IsCompleted,
                CurrentChapters = info.CurrentChapters,
                TotalChapters = info.TotalChapters,
                UpdateDate = _dateService.Today
            };

            await repository.UpsertEntityAsync(story, cancellationToken);
            await repository.UpsertEntityAsync(storyUpdate, cancellationToken);

            return new()
            {
                Value = story.ToDto()
            };
        }
    }
}
