namespace mark.davison.berlin.shared.models.dtos;

public interface IJobResponse
{
    Guid? JobId { get; set; }
    string JobStatus { get; set; }
}
