namespace mark.davison.berlin.web.features.Store.DashboardListUseCase;

public sealed class FetchDashboardListAction : BaseAction
{
    public int? Maximum { get; set; }
}

public sealed class FetchDashboardListActionResponse : BaseActionResponse<List<DashboardTileDto>>
{

}