namespace mark.davison.berlin.shared.models.Entities;

public class StorageOptions : BerlinEntity
{
    public int RetentionAmount { get; set; }
    public bool Compressed { get; set; }
}
