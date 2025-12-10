namespace mark.davison.berlin.shared.models.dto.Scenarios.Queries.Startup;

[GetRequest(Path = "startup-query")]
public sealed class StartupQueryRequest : IQuery<StartupQueryRequest, StartupQueryResponse>
{
}
