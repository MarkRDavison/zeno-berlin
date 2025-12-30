namespace mark.davison.berlin.shared.models.dto.Shared;

public sealed class StartupDataDto
{
    public List<string> EnabledAuthProviders { get; set; } = [];
    public List<UpdateTypeDto> UpdateTypes { get; set; } = [];
}
