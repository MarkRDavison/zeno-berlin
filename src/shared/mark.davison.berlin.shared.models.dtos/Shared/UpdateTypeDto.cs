namespace mark.davison.berlin.shared.models.dtos.Shared;

public sealed class UpdateTypeDto
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
}
