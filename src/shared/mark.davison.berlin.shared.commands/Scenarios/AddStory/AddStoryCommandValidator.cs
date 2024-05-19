namespace mark.davison.berlin.shared.commands.Scenarios.AddStory;

public sealed class AddStoryCommandValidator : ICommandValidator<AddStoryCommandRequest, AddStoryCommandResponse>
{
    private readonly IDbContext<BerlinDbContext> _dbContext;
    private readonly IServiceProvider _serviceProvider;

    public AddStoryCommandValidator(
        IDbContext<BerlinDbContext> dbContext,
        IServiceProvider serviceProvider
    )
    {
        _dbContext = dbContext;
        _serviceProvider = serviceProvider;
    }

    public async Task<AddStoryCommandResponse> ValidateAsync(AddStoryCommandRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.StoryAddress))
        {
            return ValidationMessages
                .CreateErrorResponse<AddStoryCommandResponse>(
                    ValidationMessages.INVALID_PROPERTY,
                    nameof(AddStoryCommandRequest.StoryAddress));
        }

        if (request.StoryAddress.StartsWith("http:"))
        {
            request.StoryAddress = request.StoryAddress.Replace("http:", "https:");
        }

        if (request.UpdateTypeId is Guid updateTypeId &&
            UpdateTypeConstants.AllIds.All(_ => _ != updateTypeId))
        {
            return ValidationMessages
                .CreateErrorResponse<AddStoryCommandResponse>(
                    ValidationMessages.INVALID_PROPERTY,
                    nameof(AddStoryCommandRequest.UpdateTypeId));
        }

        Site? site = null;

        var sites = await _dbContext
            .Set<Site>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        if (request.SiteId == null)
        {

            site = sites.FirstOrDefault(_ => request.StoryAddress.StartsWith(_.Address));

            if (site == null)
            {
                return ValidationMessages
                    .CreateErrorResponse<AddStoryCommandResponse>(
                        ValidationMessages.UNSUPPORTED_SITE);
            }
        }
        else
        {
            site = sites.FirstOrDefault(_ => _.Id == request.SiteId);
            if (site == null)
            {

                return ValidationMessages
                    .CreateErrorResponse<AddStoryCommandResponse>(
                                ValidationMessages.FAILED_TO_FIND_ENTITY,
                                nameof(Site));
            }

            if (!request.StoryAddress.StartsWith(site.Address))
            {
                return ValidationMessages
                    .CreateErrorResponse<AddStoryCommandResponse>(
                        ValidationMessages.SITE_STORY_MISMATCH);
            }
        }

        var infoProcessor = _serviceProvider.GetKeyedService<IStoryInfoProcessor>(site.ShortName);

        if (infoProcessor == null)
        {
            return ValidationMessages
                .CreateErrorResponse<AddStoryCommandResponse>(
                    ValidationMessages.UNSUPPORTED_SITE);
        }

        request.SiteId = site.Id;

        var externalId = infoProcessor.ExtractExternalStoryId(request.StoryAddress, site.Address);

        if (string.IsNullOrEmpty(externalId))
        {
            return ValidationMessages
                .CreateErrorResponse<AddStoryCommandResponse>(
                    ValidationMessages.INVALID_PROPERTY,
                    nameof(AddStoryCommandRequest.StoryAddress));
        }

        var existingStory = await _dbContext
            .Set<Story>()
            .AsNoTracking()
            .Where(_ => _.UserId == currentUserContext.CurrentUser.Id && _.ExternalId == externalId)
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
