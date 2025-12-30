namespace mark.davison.berlin.shared.models.dto;

public interface IJobRequest
{
    bool TriggerImmediateJob { get; set; }
    bool UseJob { get; set; } // Used to trigger the job
    Guid? JobId { get; set; } // Used to get the response when the job is complete...
}
