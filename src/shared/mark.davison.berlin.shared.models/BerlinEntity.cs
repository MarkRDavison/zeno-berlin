namespace mark.davison.berlin.shared.models;

public class BerlinEntity : BaseEntity
{
    public Guid UserId { get; set; }
    public virtual User? User { get; set; }
}