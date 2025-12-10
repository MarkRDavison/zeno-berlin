namespace mark.davison.berlin.api.queries.Scenarios.DashboardList;

public sealed class DashboardListQueryHandler : ValidateAndProcessQueryHandler<DashboardListQueryRequest, DashboardListQueryResponse>
{
    public DashboardListQueryHandler(
        IQueryProcessor<DashboardListQueryRequest, DashboardListQueryResponse> processor
    ) : base(
        processor)
    {
    }
}