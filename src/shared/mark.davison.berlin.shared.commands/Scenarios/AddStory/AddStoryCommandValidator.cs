namespace mark.davison.berlin.shared.commands.Scenarios.AddStory;

public class AddStoryCommandValidator : ICommandValidator<AddStoryCommandRequest, AddStoryCommandResponse>
{
    private readonly IValidationContext _validationContext;
    protected readonly IServiceProvider _serviceProvider;

    public AddStoryCommandValidator(
        IValidationContext validationContext,
        IServiceProvider serviceProvider
    )
    {
        _validationContext = validationContext;
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

        Site? site = null;
        var repository = _serviceProvider.GetRequiredService<IRepository>();
        await using (repository.BeginTransaction())
        {

            if (request.SiteId == null)
            {
                var sites = await _validationContext.GetAll<Site>(cancellationToken);

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
                site = await _validationContext.GetById<Site>(request.SiteId.Value, cancellationToken);
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

            var externalId = infoProcessor.ExtractExternalStoryId(request.StoryAddress);

            if (string.IsNullOrEmpty(externalId))
            {
                return ValidationMessages
                    .CreateErrorResponse<AddStoryCommandResponse>(
                        ValidationMessages.INVALID_PROPERTY,
                        nameof(AddStoryCommandRequest.StoryAddress));
            }

            var existingStory = await _validationContext.GetByProperty<Story, string>(
                _ => _.ExternalId == externalId,
                nameof(Story.ExternalId),
                cancellationToken);

            if (existingStory != null)
            {
                return ValidationMessages
                    .CreateErrorResponse<AddStoryCommandResponse>(
                        ValidationMessages.DUPLICATE_ENTITY,
                        nameof(Story));
            }
        }

        return new();
    }
}
