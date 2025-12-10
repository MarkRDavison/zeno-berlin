namespace mark.davison.berlin.api.commands.Scenarios.AddStory;

public sealed class AddStoryCommandValidator : ICommandValidator<AddStoryCommandRequest, AddStoryCommandResponse>
{
    private readonly IDbContext<BerlinDbContext> _dbContext;
    private readonly ISiteService _siteService;

    public AddStoryCommandValidator(
        IDbContext<BerlinDbContext> dbContext,
        ISiteService siteService
    )
    {
        _dbContext = dbContext;
        _siteService = siteService;
    }

    public async Task<AddStoryCommandResponse> ValidateAsync(AddStoryCommandRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var siteInfo = await _siteService.DetermineSiteAsync(request.StoryAddress, request.SiteId, cancellationToken);

        if (!siteInfo.Valid)
        {
            return ValidationMessages
                .CreateErrorResponse<AddStoryCommandResponse>(siteInfo.Error);
        }

        request.SiteId = siteInfo.Site.Id;
        request.StoryAddress = siteInfo.UpdatedAddress;

        var existingStory = await _dbContext
            .Set<Story>()
            .AsNoTracking()
            .Where(_ => _.UserId == currentUserContext.UserId && _.ExternalId == siteInfo.ExternalId)
            .FirstOrDefaultAsync(cancellationToken);

        if (existingStory != null)
        {
            return ValidationMessages
                .CreateErrorResponse<AddStoryCommandResponse>(
                    ValidationMessages.DUPLICATE_ENTITY,
                    nameof(Story));
        }

        return new();
    }
}
