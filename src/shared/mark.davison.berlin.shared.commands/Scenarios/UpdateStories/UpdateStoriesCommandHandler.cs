namespace mark.davison.berlin.shared.commands.Scenarios.UpdateStories;

public class UpdateStoriesCommandHandler : ValidateAndProcessCommandHandler<UpdateStoriesRequest, UpdateStoriesResponse>
{
    public UpdateStoriesCommandHandler(
        ICommandProcessor<UpdateStoriesRequest, UpdateStoriesResponse> processor
    ) : base(
        processor
    )
    {
    }
}
