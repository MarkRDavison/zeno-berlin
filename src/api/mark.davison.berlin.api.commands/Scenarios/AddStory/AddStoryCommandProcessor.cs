namespace mark.davison.berlin.api.commands.Scenarios.AddStory;

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

        var externalId = infoProcessor.ExtractExternalStoryId(request.StoryAddress, site.Address);

        var info = await infoProcessor.ExtractStoryInfo(request.StoryAddress, site.Address, cancellationToken);

        if (!info.SuccessWithValue)
        {
            if (info.Errors is { Count: > 0 })
            {
                return ValidationMessages.CreateErrorResponse<AddStoryCommandResponse>(info.Errors.First());
            }

            return ValidationMessages.CreateErrorResponse<AddStoryCommandResponse>(ValidationMessages.FAILED_RETRIEVE);
        }

        var fandoms = await _fandomService.GetOrCreateFandomsByExternalNames(info.Value.Fandoms, cancellationToken);
        var authors = await _authorService.GetOrCreateAuthorsByName(info.Value.Authors, site.Id, cancellationToken);

        var storyId = Guid.NewGuid();
        var story = new Story
        {
            Id = storyId,
            Address = infoProcessor.GenerateBaseStoryAddress(request.StoryAddress, site.Address),
            ExternalId = externalId,
            SiteId = site.Id,
            UserId = currentUserContext.UserId,
            Complete = info.Value.IsCompleted,
            CurrentChapters = info.Value.CurrentChapters,
            Name = info.Value.Name,
            TotalChapters = info.Value.TotalChapters,
            UpdateTypeId = request.UpdateTypeId ?? UpdateTypeConstants.EachChapterId,
            LastChecked = _dateService.Now,
            LastModified = _dateService.Now,
            LastAuthored = info.Value.Updated,
            Favourite = request.Favourite,
            StoryFandomLinks = [.. fandoms.Select(_ => CreateStoryFandomLink(storyId, _.Id, currentUserContext.UserId))],
            StoryAuthorLinks = [.. authors.Select(_ => CreateStoryAuthorLink(storyId, _.Id, currentUserContext.UserId))], // TODO: Some helper methods/entities/framework for linking entities
            Created = _dateService.Now
        };

        info.Value.ChapterInfo.TryGetValue(story.CurrentChapters, out var chapterInfo);
        var storyUpdate = new StoryUpdate
        {
            Id = Guid.NewGuid(),
            StoryId = story.Id,
            UserId = currentUserContext.UserId,
            Complete = info.Value.IsCompleted,
            ChapterTitle = chapterInfo?.Title,
            ChapterAddress = chapterInfo?.Address,
            CurrentChapters = info.Value.CurrentChapters,
            TotalChapters = info.Value.TotalChapters,
            LastAuthored = info.Value.Updated,
            LastModified = _dateService.Now,
            Created = _dateService.Now
        };

        await _dbContext.Set<Story>().AddAsync(story, cancellationToken);

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
    private StoryFandomLink CreateStoryFandomLink(Guid storyId, Guid fandomId, Guid userId)
    {
        return new StoryFandomLink
        {
            Id = Guid.NewGuid(),
            StoryId = storyId,
            FandomId = fandomId,
            UserId = userId,
            Created = _dateService.Now,
            LastModified = _dateService.Now
        };
    }
    // TODO: Duplicate
    private StoryAuthorLink CreateStoryAuthorLink(Guid storyId, Guid authorId, Guid userId)
    {
        return new StoryAuthorLink
        {
            Id = Guid.NewGuid(),
            StoryId = storyId,
            AuthorId = authorId,
            UserId = userId,
            Created = _dateService.Now,
            LastModified = _dateService.Now
        };
    }
}
