﻿namespace mark.davison.berlin.shared.models.dtos.Scenarios.Commands.Import;

[PostRequest(Path = "import-command")]
public sealed class ImportCommandRequest : ICommand<ImportCommandRequest, ImportCommandResponse>, IJobRequest
{
    public SerialisedtDataDto Data { get; set; } = new();
    public bool TriggerImmediateJob { get; set; }
    public bool UseJob { get; set; }
    public Guid? JobId { get; set; }
}
