namespace mark.davison.berlin.shared.models.dtos.Scenarios.Queries.DashboardList;

[GetRequest(Path = "dashboard-list-query")]
public sealed class DashboardListQueryRequest : IQuery<DashboardListQueryRequest, DashboardListQueryResponse>
{
    public int? Maximum { get; set; }
}
