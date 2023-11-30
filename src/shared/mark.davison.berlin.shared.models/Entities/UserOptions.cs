namespace mark.davison.berlin.shared.models.Entities;

public class UserOptions : BerlinEntity
{
    public long MaxCapacity { get; set; }
    public bool IsAdmin { get; set; }
}
