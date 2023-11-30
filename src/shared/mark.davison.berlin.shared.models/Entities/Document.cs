namespace mark.davison.berlin.shared.models.Entities;

public class Document : BerlinEntity
{
    public string Name { get; set; } = string.Empty;
    public string FullPath { get; set; } = string.Empty;
    public int Version { get; set; }
    public bool IsBackup { get; set; }
    public Guid LocationId { get; set; }
    public Guid ShareId { get; set; }
    public Guid? SharingOptionsId { get; set; }
    public Guid? StorageOptionsId { get; set; }

    public virtual Location? Location { get; set; }
    public virtual Share? Share { get; set; }
    public virtual SharingOptions? SharingOptions { get; set; }
    public virtual StorageOptions? StorageOptions { get; set; }
}
