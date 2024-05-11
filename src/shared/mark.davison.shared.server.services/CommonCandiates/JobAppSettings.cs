namespace mark.davison.shared.server.services.CommonCandiates;

public class JobAppSettings : IAppSettings
{
    public string SECTION => "JOBS";
    public string JOB_CHECK_EVENT_KEY { get; set; } = string.Empty;
}
