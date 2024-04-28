namespace mark.davison.berlin.shared.models.dtos.Shared;

public class AuthorDto
{
    public Guid AuthorId { get; set; }
    public Guid? ParentAuthorId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsUserSpecified { get; set; }
}
