namespace mark.davison.berlin.shared.commands.Scenarios.AddPotentialStory;

public sealed class AddPotentialStoryCommandProcessor : ICommandProcessor<AddPotentialStoryCommandRequest, AddPotentialStoryCommandResponse>
{
    private readonly IDbContext _dbContext;
    private readonly IServiceProvider _serviceProvider;

    public AddPotentialStoryCommandProcessor(
        IDbContext dbContext,
        IServiceProvider serviceProvider)
    {
        _dbContext = dbContext;
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

        var entity = new PotentialStory
        {
            Id = Guid.NewGuid(),
            Name = info.Name,
            Summary = info.Summary,
            Address = request.StoryAddress,
            UserId = currentUserContext.CurrentUser.Id
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
