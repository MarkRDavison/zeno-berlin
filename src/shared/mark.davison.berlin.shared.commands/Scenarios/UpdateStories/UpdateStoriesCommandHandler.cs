namespace mark.davison.berlin.shared.commands.Scenarios.UpdateStories;

public sealed class UpdateStoriesCommandHandler : ValidateAndProcessCommandHandler<UpdateStoriesRequest, UpdateStoriesResponse>
{
    public UpdateStoriesCommandHandler(
        ICommandProcessor<UpdateStoriesRequest, UpdateStoriesResponse> processor
    ) : base(
        processor
    )
    {
    }
}
