namespace mark.davison.berlin.shared.commands.Scenarios.AddStory;

public class AddStoryCommandProcessor : ICommandProcessor<AddStoryCommandRequest, AddStoryCommandResponse>
{
    private readonly IDateService _dateService;
    private readonly IValidationContext _validationContext;
    private readonly IFandomService _fandomService;
    private readonly IServiceProvider _serviceProvider;

    public AddStoryCommandProcessor(
        IDateService dateService,
        IValidationContext validationContext,
        IFandomService fandomService,
        IServiceProvider serviceProvider
    )
    {
        _dateService = dateService;
        _validationContext = validationContext;
        _fandomService = fandomService;
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

            var fandoms = await _fandomService.GetOrCreateFandomsByExternalNames(info.Fandoms, cancellationToken);

            var storyId = Guid.NewGuid();
            var story = new Story
            {
                Id = storyId,
                Address = infoProcessor.GenerateBaseStoryAddress(request.StoryAddress),
                ExternalId = externalId,
                SiteId = site.Id,
                UserId = currentUserContext.CurrentUser.Id,
                Complete = info.IsCompleted,
                CurrentChapters = info.CurrentChapters,
                Name = info.Name,
                TotalChapters = info.TotalChapters,
                UpdateTypeId = UpdateTypeConstants.EachChapterId,
                LastChecked = _dateService.Now,
                LastModified = _dateService.Now,
                LastAuthored = info.Updated,
                Favourite = request.Favourite,
                StoryFandomLinks = [.. fandoms.Select(_ => CreateStoryFandomLink(storyId, _.Id, currentUserContext.CurrentUser.Id))]
            };

            var storyUpdate = new StoryUpdate
            {
                Id = Guid.NewGuid(),
                StoryId = story.Id,
                UserId = currentUserContext.CurrentUser.Id,
                Complete = info.IsCompleted,
                CurrentChapters = info.CurrentChapters,
                TotalChapters = info.TotalChapters,
                LastAuthored = info.Updated,
                LastModified = _dateService.Now,
            };

            await repository.UpsertEntityAsync(story, cancellationToken);

            if (!request.SuppressUpdateCreation)
            {
                await repository.UpsertEntityAsync(storyUpdate, cancellationToken);
            }

            return new()
            {
                Value = story.ToDto()
            };
        }
    }

    // TODO: Duplicate
    private static StoryFandomLink CreateStoryFandomLink(Guid storyId, Guid fandomId, Guid userId)
    {
        return new StoryFandomLink
        {
            Id = Guid.NewGuid(),
            StoryId = storyId,
            FandomId = fandomId,
            UserId = userId
        };
    }
}
