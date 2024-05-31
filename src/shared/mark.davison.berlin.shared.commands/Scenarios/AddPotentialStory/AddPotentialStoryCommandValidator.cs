namespace mark.davison.berlin.shared.commands.Scenarios.AddPotentialStory;

public sealed class AddPotentialStoryCommandValidator : ICommandValidator<AddPotentialStoryCommandRequest, AddPotentialStoryCommandResponse>
{
    private readonly IDbContext _dbContext;
    private readonly ISiteService _siteService;

    public AddPotentialStoryCommandValidator(
        IDbContext dbContext,
        ISiteService siteService)
    {
        _dbContext = dbContext;
        _siteService = siteService;
    }

    public async Task<AddPotentialStoryCommandResponse> ValidateAsync(AddPotentialStoryCommandRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var siteInfo = await _siteService.DetermineSiteAsync(request.StoryAddress, request.SiteId, cancellationToken);

        if (!siteInfo.Valid)
        {
            return ValidationMessages
                .CreateErrorResponse<AddPotentialStoryCommandResponse>(siteInfo.Error);
        }

        request.SiteId = siteInfo.Site.Id;
        request.StoryAddress = siteInfo.UpdatedAddress;

        var duplicateStoryExists = await _dbContext
            .AnyAsync<Story>(_ =>
                _.UserId == currentUserContext.CurrentUser.Id &&
                _.ExternalId == siteInfo.ExternalId, cancellationToken);

        if (duplicateStoryExists)
        {
            return ValidationMessages.CreateErrorResponse<AddPotentialStoryCommandResponse>(
                ValidationMessages.DUPLICATE_ENTITY,
                nameof(Story),
                nameof(Story.Address));
        }

        var duplicatePotentialStoryExists = await _dbContext
            .AnyAsync<PotentialStory>(_ =>
                _.UserId == currentUserContext.CurrentUser.Id &&
                _.Address == request.StoryAddress, cancellationToken);

        if (duplicatePotentialStoryExists)
        {
            return ValidationMessages.CreateErrorResponse<AddPotentialStoryCommandResponse>(
                ValidationMessages.DUPLICATE_ENTITY,
                nameof(PotentialStory),
                nameof(PotentialStory.Address));
        }

        return new();
    }
}
