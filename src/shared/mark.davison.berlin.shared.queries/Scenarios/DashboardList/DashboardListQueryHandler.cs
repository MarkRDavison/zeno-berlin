
namespace mark.davison.berlin.shared.queries.Scenarios.DashboardList;

public sealed class DashboardListQueryHandler : ValidateAndProcessQueryHandler<DashboardListQueryRequest, DashboardListQueryResponse>
{
    public DashboardListQueryHandler(
        IQueryProcessor<DashboardListQueryRequest, DashboardListQueryResponse> processor
    ) : base(
        processor)
    {
    }
}