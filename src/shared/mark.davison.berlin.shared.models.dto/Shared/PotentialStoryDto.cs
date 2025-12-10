namespace mark.davison.berlin.shared.models.dto.Shared;

public sealed class PotentialStoryDto
{
    public Guid Id { get; set; }
    public string Address { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
}
