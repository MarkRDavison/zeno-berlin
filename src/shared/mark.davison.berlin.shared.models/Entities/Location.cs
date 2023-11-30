namespace mark.davison.berlin.shared.models.Entities;

public class Location : BerlinEntity
{
    public string Path { get; set; } = string.Empty;
    public Guid? SharingOptionsId { get; set; }
    public Guid? StorageOptionsId { get; set; }

    public virtual SharingOptions? SharingOptions { get; set; }
    public virtual StorageOptions? StorageOptions { get; set; }
}
