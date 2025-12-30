namespace mark.davison.berlin.shared.models.dto;

public interface IJobResponse
{
    Guid? JobId { get; set; }
    string JobStatus { get; set; }
}
