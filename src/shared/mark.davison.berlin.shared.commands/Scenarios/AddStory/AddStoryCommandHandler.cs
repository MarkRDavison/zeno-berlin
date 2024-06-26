﻿namespace mark.davison.berlin.shared.commands.Scenarios.AddStory;

public sealed class AddStoryCommandHandler : ValidateAndProcessCommandHandler<AddStoryCommandRequest, AddStoryCommandResponse>
{
    public AddStoryCommandHandler(
        ICommandProcessor<AddStoryCommandRequest, AddStoryCommandResponse> processor,
        ICommandValidator<AddStoryCommandRequest, AddStoryCommandResponse> validator
    ) : base(
        processor,
        validator)
    {
    }
}
