namespace mark.davison.berlin.shared.commands.Scenarios.AddStory;

public sealed class AddStoryCommandProcessor : ICommandProcessor<AddStoryCommandRequest, AddStoryCommandResponse>
{
    private readonly IDateService _dateService;
    private readonly IDbContext<BerlinDbContext> _dbContext;
    private readonly IFandomService _fandomService;
    private readonly IAuthorService _authorService;
    private readonly IServiceProvider _serviceProvider;

    public AddStoryCommandProcessor(
        IDateService dateService,
        IDbContext<BerlinDbContext> dbContext,
        IFandomService fandomService,
        IAuthorService authorService,
        IServiceProvider serviceProvider
    )
    {
        _dateService = dateService;
        _dbContext = dbContext;
        _fandomService = fandomService;
        _authorService = authorService;
        _serviceProvider = serviceProvider;
    }

    public async Task<AddStoryCommandResponse> ProcessAsync(AddStoryCommandRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var site = await _dbContext
            .Set<Site>()
            .AsNoTracking()
            .Where(_ => _.Id == request.SiteId!.Value)
            .SingleAsync(cancellationToken);

        var infoProcessor = _serviceProvider.GetRequiredKeyedService<IStoryInfoProcessor>(site!.ShortName);

        var externalId = infoProcessor.ExtractExternalStoryId(request.StoryAddress);

        var info = await infoProcessor.ExtractStoryInfo(request.StoryAddress, cancellationToken);

        var fandoms = await _fandomService.GetOrCreateFandomsByExternalNames(info.Fandoms, cancellationToken);
        var authors = await _authorService.GetOrCreateAuthorsByName(info.Authors, site.Id, cancellationToken);

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
            UpdateTypeId = request.UpdateTypeId ?? UpdateTypeConstants.EachChapterId,
            LastChecked = _dateService.Now,
            LastModified = _dateService.Now,
            LastAuthored = info.Updated,
            Favourite = request.Favourite,
            StoryFandomLinks = [.. fandoms.Select(_ => CreateStoryFandomLink(storyId, _.Id, currentUserContext.CurrentUser.Id))],
            StoryAuthorLinks = [.. authors.Select(_ => CreateStoryAuthorLink(storyId, _.Id, currentUserContext.CurrentUser.Id))] // TODO: Some helper methods/entities/framework for linking entities
        };

        info.ChapterInfo.TryGetValue(story.CurrentChapters, out var chapterInfo);
        var storyUpdate = new StoryUpdate
        {
            Id = Guid.NewGuid(),
            StoryId = story.Id,
            UserId = currentUserContext.CurrentUser.Id,
            Complete = info.IsCompleted,
            ChapterTitle = chapterInfo?.Title,
            ChapterAddress = chapterInfo?.Address,
            CurrentChapters = info.CurrentChapters,
            TotalChapters = info.TotalChapters,
            LastAuthored = info.Updated,
            LastModified = _dateService.Now,
        };

        await _dbContext.Set<Story>().AddAsync(story);

        if (!request.SuppressUpdateCreation)
        {
            await _dbContext.Set<StoryUpdate>().AddAsync(storyUpdate, cancellationToken);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new()
        {
            Value = story.ToDto()
        };
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
    // TODO: Duplicate
    private static StoryAuthorLink CreateStoryAuthorLink(Guid storyId, Guid authorId, Guid userId)
    {
        return new StoryAuthorLink
        {
            Id = Guid.NewGuid(),
            StoryId = storyId,
            AuthorId = authorId,
            UserId = userId
        };
    }
}
