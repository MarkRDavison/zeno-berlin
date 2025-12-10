namespace mark.davison.berlin.api.commands.Scenarios.AddPotentialStory;

public sealed class AddPotentialStoryCommandProcessor : ICommandProcessor<AddPotentialStoryCommandRequest, AddPotentialStoryCommandResponse>
{
    private readonly IDbContext _dbContext;
    private readonly IDateService _dateService;
    private readonly IServiceProvider _serviceProvider;

    public AddPotentialStoryCommandProcessor(
        IDbContext dbContext,
        IDateService dateService,
        IServiceProvider serviceProvider)
    {
        _dbContext = dbContext;
        _dateService = dateService;
        _serviceProvider = serviceProvider;
    }

    public async Task<AddPotentialStoryCommandResponse> ProcessAsync(AddPotentialStoryCommandRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var site = await _dbContext
            .Set<Site>()
            .AsNoTracking()
            .Where(_ => _.Id == request.SiteId!.Value)
            .SingleAsync(cancellationToken);

        var infoProcessor = _serviceProvider.GetRequiredKeyedService<IStoryInfoProcessor>(site!.ShortName);

        var info = await infoProcessor.ExtractStoryInfo(request.StoryAddress, site.Address, cancellationToken);

        if (info is null)
        {
            return ValidationMessages.CreateErrorResponse<AddPotentialStoryCommandResponse>(ValidationMessages.FAILED_RETRIEVE);
        }

        var entity = new PotentialStory
        {
            Id = Guid.NewGuid(),
            Name = info.Name,
            Summary = info.Summary,
            Address = request.StoryAddress,
            UserId = currentUserContext.UserId,
            Created = _dateService.Now,
            LastModified = _dateService.Now
        };

        await _dbContext.UpsertEntityAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new AddPotentialStoryCommandResponse
        {
            Value = new PotentialStoryDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Summary = entity.Summary,
                Address = entity.Address
            }
        };
    }
}
