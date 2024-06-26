﻿namespace mark.davison.berlin.shared.commands.Scenarios.AddFandom;

public sealed class AddFandomCommandHandler : ValidateAndProcessCommandHandler<AddFandomCommandRequest, AddFandomCommandResponse>
{
    public AddFandomCommandHandler(
        ICommandProcessor<AddFandomCommandRequest, AddFandomCommandResponse> processor,
        ICommandValidator<AddFandomCommandRequest, AddFandomCommandResponse> validator
    ) : base(
        processor,
        validator)
    {
    }
}
