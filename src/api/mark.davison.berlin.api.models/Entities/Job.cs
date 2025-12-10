namespace mark.davison.berlin.api.models.Entities;

public class Job : BerlinEntity
{
    public string JobType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string JobRequest { get; set; } = string.Empty;
    public string JobResponse { get; set; } = string.Empty;

    public DateTime SubmittedAt { get; set; }
    public DateTime SelectedAt { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime FinishedAt { get; set; }

    public string PerformerId { get; set; } = string.Empty;
    public Guid ContextUserId { get; set; }

    public virtual User? ContextUser { get; set; }
}
