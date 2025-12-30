namespace mark.davison.berlin.api.models.Entities;

public class Site : BerlinEntity
{
    public string ShortName { get; set; } = string.Empty;
    public string LongName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}
